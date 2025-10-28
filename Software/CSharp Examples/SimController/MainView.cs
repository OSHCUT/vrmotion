using System.Globalization;
using TcpText;

namespace SimController
{
    public partial class MainView : Form
    {
        private TcpTextServer? _remoteControlServer;
        private readonly UdpReceiver _receiver;
        private MotorInterface? _motorInterface;
        private System.Windows.Forms.Timer _pollTimer;
        private System.Windows.Forms.Timer _telemetryFailsafeTimer;
        private System.Windows.Forms.Timer _remoteControlFailsafeTimer;

        private double yaw, pitch, roll;
        private double yawRate, pitchRate, rollRate;
        private double heave, surge, sway;

        // Degrees / rotation, divided by gearbox ratio, divided by output pulley ratio, divided by encoder counts per revolution
        private const double pitchCountsToDegrees = 360 / 7.5 / (35.06 / 2.228) / 32000;
        private const double rollCountsToDegrees = 360 / 7.5 / (35.06 / 2.228) / 32000;
        private const double yawCountsToDegrees = 360 / 15 / (19.099 / 2.228) / 32000;

        private const double pitchDegreesToCounts = 1 / pitchCountsToDegrees;
        private const double rollDegreesToCounts = 1 / rollCountsToDegrees;
        private const double yawDegreesToCounts = 1 / yawCountsToDegrees;

        private const double pitchMaxCommandedDegrees = 40;
        private const double rollMaxCommandedDegrees = 44;
        private const double rollMaxCommandedDegreesPerSecond = 51; // These max rates roughly match 1000 RPM for the motors. Can got higher, but at limited torque.
        private const double pitchMaxCommandedDegreesPerSecond = 51;
        private const double yawMaxCommandedDegreesPerSecond = 47;
        private const long pitchMaxCommandedCounts = (long)(pitchMaxCommandedDegrees * pitchDegreesToCounts);
        private const long rollMaxCommandedCounts = (long)(rollMaxCommandedDegrees * rollDegreesToCounts);
        private const long yawMaxCommandedCountsPerSecond = (long)(yawMaxCommandedDegreesPerSecond * yawDegreesToCounts);
        private const long rollMaxCommandedCountsPerSecond = (long)(rollMaxCommandedDegreesPerSecond * rollDegreesToCounts);
        private const long pitchMaxCommandedCountsPerSecond = (long)(pitchMaxCommandedDegreesPerSecond * pitchDegreesToCounts);

        private long yawZeroCounts = 0;
        private double yawScale = 1.0;  // Affects how accurately yaw rate matches telemetry data. Should range from 0.1 to 1.0.
        private double rollScale = 1.0; // Scale for roll position, affects how exactly roll matches telemetry data. Should range from 0.1 to 1.0. Numbers < 1.0 increase simulation "dynamic range"
        private double pitchScale = 1.0; // Scale for pitch position, affects how exactly pitch matches telemetry data. Should range from 0.1 to 1.0.

        private int commandedYawCounts = 0;
        private double commandedYawRateCountsPerSecond = 0;

        private int commandedRollCounts = 0;
        private double commandedRollRateCountsPerSecond = 0;

        private int commandedPitchCounts = 0;
        private double commandedPitchRateCountsPerSecond = 0;

        private bool remoteControlConnected = false;
        private bool telemetryMotionEnabled = false; // If true, telemetry data is used to control motors.
        private SimulatorState? simulatorState;

        const double pitchAndRollDriftCorrectFactor = 4.0; // A small bias to correct for drift in pitch and roll positions over time.
        private bool telemetryStreamActive = false; // True if we've receive a telemetry packet in the last 100 milliseconds. False otherwise.

        private bool telemetryIsRadians = false;
        private bool mixAccelerations = true;
        private double surgeToPitchFactor = 2;
        private double swayToRollFactor = 2;

        public MainView()
        {
            InitializeComponent();

            this.Load += StartRemoteControlServer;

            _receiver = new UdpReceiver(5123);
            _receiver.MessageReceived += OnUdpMessageReceived;
            _receiver.Start();

            _motorInterface = new MotorInterface();
            _motorInterface.StatusChanged += OnSimStatusChanged;
            _motorInterface.StateChanged += OnSimStateChanged;
            _motorInterface.Start();

            _pollTimer = new System.Windows.Forms.Timer();
            _pollTimer.Interval = 200; // milliseconds => 5 Hz
            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();

            _telemetryFailsafeTimer = new System.Windows.Forms.Timer();
            _telemetryFailsafeTimer.Interval = 100;
            _telemetryFailsafeTimer.Tick += TelemetryFailsafeTimer_Tick;
            _telemetryFailsafeTimer.Start();

            _remoteControlFailsafeTimer = new System.Windows.Forms.Timer();
            _remoteControlFailsafeTimer.Interval = 500;
            _remoteControlFailsafeTimer.Tick += RemoteControlFailsafeTimer_Tick;
            _remoteControlFailsafeTimer.Start();

            trackBarYawScale.Value = (int)Math.Round(yawScale * 100);
            yawScaleLabel.Text = yawScale.ToString("F2");

            trackBarPitchScale.Value = (int)Math.Round(pitchScale * 100);
            pitchScaleLabel.Text = pitchScale.ToString("F2");

            trackBarRollScale.Value = (int)Math.Round(rollScale * 100);
            rollScaleLabel.Text = rollScale.ToString("F2");
        }
        private async void StartRemoteControlServer(object? sender, EventArgs e)
        {
            if (_remoteControlServer != null)
            {
                _remoteControlServer.Dispose();
            }

            _remoteControlServer = new TcpTextServer(this);
            _remoteControlServer.StatusChanged += (_, msg) => OnRemoteControlStatusChanged(msg);
            _remoteControlServer.MessageReceived += (_, line) => OnRemoteControlMessageReceived(line);
            await _remoteControlServer.StartAsync(port: 5555);
        }

        private void PollTimer_Tick(object? sender, EventArgs e)
        {
            if (simulatorState != null && simulatorState.portConnected)
            {
                SimulatorCommand pollCmd = new SimulatorCommand { Name = "GetState" };
                _motorInterface?.EnqueueCommand(pollCmd);
            }
        }

        private void RemoteControlFailsafeTimer_Tick(object? sender, EventArgs e)
        {
            if (remoteControlConnected && _motorInterface != null && simulatorState != null && simulatorState.portConnected && telemetryMotionEnabled)
            {
                telemetryMotionEnabled = false;

                FailsafeTriggered();
            }
        }

        private void TelemetryFailsafeTimer_Tick(object? sender, EventArgs e)
        {
            telemetryStreamActive = false;
            // If we haven't received a telemetry packet in the last 100 milliseconds, stop the motors.
            if (_motorInterface != null && simulatorState != null && simulatorState.portConnected && telemetryMotionEnabled)
            {
                FailsafeTriggered();
            }
            else
            {
                telemetryStatusLabel.Text = "Inactive - No Telemetry Received";
            }
            UpdateButtonStates();
        }

        private void FailsafeTriggered()
        {
            SimulatorCommand cmd = new SimulatorCommand { Name = "StartUnmonitoredMove" };
            cmd.Data.yawRateCountsPerSecond = 0;
            cmd.Data.pitchRateCountsPerSecond = 0;
            cmd.Data.rollRateCountsPerSecond = 0;
            cmd.Data.isVelocityCommand = true;
            _motorInterface.EnqueueCommand(cmd);
            telemetryMotionEnabled = false;
            telemetryStatusLabel.Text = "ERROR: Failed to receive telemetry data in 100ms, motion disabled.";

            // Turn off the motors!
            SimulatorCommand cmd2 = new SimulatorCommand { Name = "DisableMotors" };
            _motorInterface.EnqueueCommand(cmd2);
        }

        private void ReportTelemetryReceived()
        {
            _telemetryFailsafeTimer.Stop();
            telemetryStreamActive = true;
            _telemetryFailsafeTimer.Start();
            telemetryStatusLabel.Text = "Telemetry active.";
        }

        private void OnRemoteControlStatusChanged(string message)
        {
            if (message.StartsWith("Client connected"))
            {
                remoteControlConnected = true;
                remoteControlStatusLabel.Text = "Remote Control: Connected";
            }
            else if (message.StartsWith("Client disconnected") || message.StartsWith("Disconnected") || message.StartsWith("Server stopped"))
            {
                remoteControlConnected = false;
                remoteControlStatusLabel.Text = "";
            }
        }

        private async void OnRemoteControlMessageReceived(string message)
        {
            _remoteControlFailsafeTimer.Stop();
            _remoteControlFailsafeTimer.Start();

            if (!message.StartsWith("KEEPALIVE"))
            {
                labelLastRemoteCommand.Text = "Last CMD: " + message;
            }

            if (message.IndexOf("ZERO_AXES") >= 0)
            {
                SimulatorCommand cmd = new SimulatorCommand { Name = "ZeroAllMotors" };
                _motorInterface?.EnqueueCommand(cmd);
            }
            else if (message.IndexOf("STOP_SIMULATION") >= 0)
            {
                StopSimulationMotion();
            }
            else if (message.IndexOf("START_SIMULATION") >= 0)
            {
                StartSimulationMotion();
            }
            else if (message.IndexOf("GO_HOME") >= 0)
            {
                SimulatorCommand cmd = new SimulatorCommand { Name = "GotoZero" };
                _motorInterface?.EnqueueCommand(cmd);
            }

            if (_remoteControlServer == null || !_remoteControlServer.IsConnected)
            {
                return;
            }

            string simStateJson = "{" +
                $"\"readyToMove\": {((simulatorState != null && simulatorState.portConnected && !simulatorState.homingInProgress && !simulatorState.movingToZero) ? "true" : "false")}," +
                $"\"telemetryStreamActive\": {telemetryStreamActive.ToString().ToLower() ?? "false"}," +
                $"\"telemetryMotionEnabled\": {telemetryMotionEnabled.ToString().ToLower() ?? "false"}," +
                $"\"isHomed\": {simulatorState?.motorsHomed.ToString().ToLower() ?? "false"}," +
                $"\"isHoming\": {simulatorState?.homingInProgress.ToString().ToLower() ?? "false"}" + 
                $"\"isMovingToZero\": {simulatorState?.movingToZero.ToString().ToLower() ?? "false"}" +
                "}";

            try
            {
                await _remoteControlServer.SendAsync(simStateJson);
            } catch (Exception ex)
            {
            }            
        }

        private void OnSimStatusChanged(string message)
        {
            simHubStatusLabel.Text = "Status: " + message;
        }

        // This exposes a private member variables and I'm not sure if the compiler cares.
        private void OnSimStateChanged(SimulatorState simState)
        {
            simulatorState = simState;
            UpdateButtonStates();

            // Update motor state values
            simYawLabel.Text = simulatorState.yawRate.ToString("F1");
            simPitchLabel.Text = simulatorState.pitchCounts.ToString("F1");
            simRollLabel.Text = simulatorState.rollCounts.ToString("F1");
        }

        private void UpdateButtonStates()
        {
            // Disable all buttons until the simulator is sending us data, showing we are connected.
            if (simulatorState == null)
            {
                simStartStopHomingButton.Enabled = false;
                simGoToZeroButton.Enabled = false;
                enableTelemetryLinkButton.Enabled = false;

                return;
            }

            if (telemetryMotionEnabled)
            {
                trackBarYawScale.Enabled = false;
                trackBarRollScale.Enabled = false;
                trackBarPitchScale.Enabled = false;
            } else
            {
                trackBarYawScale.Enabled = true;
                trackBarRollScale.Enabled = true;
                trackBarPitchScale.Enabled = true;
            }

            if (simulatorState.motorsHomed)
            {
                homingStatusLabel.Text = "Homed";
            }
            else if (simulatorState.homingInProgress)
            {
                homingStatusLabel.Text = "Homing in Progress...";
            }
            else
            {
                homingStatusLabel.Text = "Not Homed";
            }

            if (!simulatorState.homingInProgress && !simulatorState.movingToZero && !telemetryMotionEnabled)
            {
                simStartStopHomingButton.Enabled = true;
                simGoToZeroButton.Enabled = true;
            }
            else
            {
                simStartStopHomingButton.Enabled = false;
                simGoToZeroButton.Enabled = false;
            }

            if (telemetryStreamActive && 
                (!simulatorState.homingInProgress && !simulatorState.movingToZero))
            {
                enableTelemetryLinkButton.Enabled = true;
            }
            else
            {
                enableTelemetryLinkButton.Enabled = false;
            }

            if (telemetryMotionEnabled)
            {
                enableTelemetryLinkButton.Text = "Disable Motion";
            }
            else
            {
                enableTelemetryLinkButton.Text = "Enable Motion";
            }
        }
        private void OnUdpMessageReceived(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateTelemetryData(message));
            }
            else
            {
                UpdateTelemetryData(message);
            }
        }

        private void UpdateTelemetryData(string message)
        {
            telemetryRawPacketLabel.Text = message;

            string[] tokens = message.Split(',');

            if (tokens.Length != 11 || tokens[0] != "START" || tokens[^1] != "END")
            {
                return;
            }

            ReportTelemetryReceived();

            if (simulatorState == null)
            {
                return;
            }

            uint yawRaw, pitchRaw, rollRaw;
            uint yawRateRaw, pitchRateRaw, rollRateRaw;
            uint heaveRaw, surgeRaw, swayRaw;

            if (!uint.TryParse(tokens[1], NumberStyles.None, CultureInfo.InvariantCulture, out yawRaw)) return;
            if (!uint.TryParse(tokens[2], NumberStyles.None, CultureInfo.InvariantCulture, out rollRaw)) return;
            if (!uint.TryParse(tokens[3], NumberStyles.None, CultureInfo.InvariantCulture, out pitchRaw)) return;
            if (!uint.TryParse(tokens[4], NumberStyles.None, CultureInfo.InvariantCulture, out heaveRaw)) return;
            if (!uint.TryParse(tokens[5], NumberStyles.None, CultureInfo.InvariantCulture, out swayRaw)) return;
            if (!uint.TryParse(tokens[6], NumberStyles.None, CultureInfo.InvariantCulture, out surgeRaw)) return;
            if (!uint.TryParse(tokens[7], NumberStyles.None, CultureInfo.InvariantCulture, out yawRateRaw)) return;
            if (!uint.TryParse(tokens[8], NumberStyles.None, CultureInfo.InvariantCulture, out rollRateRaw)) return;
            if (!uint.TryParse(tokens[9], NumberStyles.None, CultureInfo.InvariantCulture, out pitchRateRaw)) return;

            // These divisors and multipliers are based on the range of values expected from SimTools telemetry.
            // I've set SimTools up to output 16 bit unsigned integers, which gives a range of 0 to 16777216.
            // I've also set up the min/max value ranges from -180 to 180 degrees for yaw, roll, and pitch,
            // and -10 to 10 for heave, sway, and surge, and -90 to 90 degrees per second for yaw, roll, and pitch rates.
            // This yields the conversions below.
            yaw = (yawRaw / 65536.0) * 360.0 - 180.0;
            roll = (rollRaw / 65536.0) * 360.0 - 180.0;
            pitch = (pitchRaw / 65536.0) * 360.0 - 180.0;
            heave = (heaveRaw / 65536.0) * 20.0 - 10.0;
            sway = (swayRaw / 65536.0) * 20.0 - 10.0;
            surge = (surgeRaw / 65536.0) * 20.0 - 10.0;
            yawRate = (yawRateRaw / 65536.0) * 180 - 90.0;
            rollRate = (rollRateRaw / 65536.0) * 180 - 90.0;
            pitchRate = (pitchRateRaw / 65536.0) * 180 - 90.0;

            if (telemetryIsRadians)
            {
                yaw = yaw * (Math.PI / 180);
                roll = roll * (Math.PI / 180);
                pitch = pitch * (Math.PI / 180);
                yawRate = yawRate * (Math.PI / 180);
                rollRate = rollRate * (Math.PI / 180);
                pitchRate = pitchRate * (Math.PI / 180);
            }

            if (mixAccelerations)
            {
                roll -= sway * swayToRollFactor;
                pitch -= surge * surgeToPitchFactor;
            }

            telemetryYawLabel.Text = $"{yaw:F2}°";
            telemetryRollLabel.Text = $"{roll:F2}°";
            telemetryPitchLabel.Text = $"{pitch:F2}°";
            telemetryHeaveLabel.Text = $"{heave:F2}";
            telemetrySwayLabel.Text = $"{sway:F2}";
            telemetrySurgeLabel.Text = $"{surge:F2}";
            telemetryYawRateLabel.Text = $"{yawRate:F2}°/s";
            telemetryRollRateLabel.Text = $"{rollRate:F2}°/s";
            telemetryPitchRateLabel.Text = $"{pitchRate:F2}°/s";

            commandedYawCounts = (int)(yaw * yawDegreesToCounts - yawZeroCounts);  // TODO: Deal with rollover at 180 degrees

            if (pitch >= 0)
            {
                commandedPitchCounts = (int)Math.Min(pitch * pitchDegreesToCounts * pitchScale, pitchMaxCommandedCounts);
            }
            else
            {
                commandedPitchCounts = (int)Math.Max(pitch * pitchDegreesToCounts * pitchScale, -pitchMaxCommandedCounts);
            }

            if (roll >= 0)
            {
                commandedRollCounts = (int)Math.Min(roll * rollDegreesToCounts * rollScale, rollMaxCommandedCounts);
            }
            else
            {
                commandedRollCounts = (int)Math.Max(roll * rollDegreesToCounts * rollScale, -rollMaxCommandedCounts);
            }

            // Rates
            if (yawRate >= 0)
            {
                commandedYawRateCountsPerSecond = Math.Min(yawRate * yawDegreesToCounts * yawScale, yawMaxCommandedCountsPerSecond);
            }
            else
            {
                commandedYawRateCountsPerSecond = Math.Max(yawRate * yawDegreesToCounts * yawScale, -yawMaxCommandedCountsPerSecond);
            }

            // Adds a very slight bias to roll rate to correct any long-term drift in the roll position, since we are commanding rates,
            // not positions.
            double simRoll = simulatorState.rollCounts * rollCountsToDegrees;
            double adjustedRollRate;
            if (roll >= 0)
            {
                adjustedRollRate = (rollRate + pitchAndRollDriftCorrectFactor * (Math.Min(roll, rollMaxCommandedDegrees) - simRoll)) * rollDegreesToCounts * rollScale;
            }
            else
            {
                adjustedRollRate = (rollRate + pitchAndRollDriftCorrectFactor * (Math.Max(roll, -rollMaxCommandedDegrees) - simRoll)) * rollDegreesToCounts * rollScale;
            }

            // As we approach physical motion limits, alter allowed roll rates to prevent crashing.
            if (simRoll >= 0)
            {
                // If at or beyond max supported roll, only allow negative roll rates.
                if (rollMaxCommandedDegrees <= simRoll)
                {
                    adjustedRollRate = Math.Min(adjustedRollRate, 0);
                }
                // If getting close, ramp the roll rate down to zero as we approach the max roll.
                else if (rollMaxCommandedDegrees - simRoll < 10)
                {
                    adjustedRollRate = Math.Min(adjustedRollRate, rollMaxCommandedDegreesPerSecond * (rollMaxCommandedDegrees - simRoll) / 10);
                }
            }
            else
            {
                // If at or beyond min supported roll, only allow positive roll rates.
                if (simRoll <= -rollMaxCommandedDegrees)
                {
                    adjustedRollRate = Math.Max(adjustedRollRate, 0);
                }
                // If getting close, ramp the roll rate down to zero as we approach the min roll.
                else if (rollMaxCommandedDegrees + simRoll < 10)
                {
                    adjustedRollRate = Math.Max(adjustedRollRate, -rollMaxCommandedDegreesPerSecond * (simRoll + rollMaxCommandedDegrees) / 10);
                }
            }

            // Now threshold against min and max commanded roll rates for normal operation.
            if (adjustedRollRate >= 0)
            {
                commandedRollRateCountsPerSecond = (int)Math.Min(adjustedRollRate, rollMaxCommandedCountsPerSecond);
            }
            else
            {
                commandedRollRateCountsPerSecond = (int)Math.Max(adjustedRollRate, -rollMaxCommandedCountsPerSecond);
            }

            // If the telemetry is calling for something outside our dynamic range, command zero velocity.
            if (roll * rollScale >= rollMaxCommandedDegrees || roll * rollScale <= -rollMaxCommandedDegrees)
            {
                commandedRollRateCountsPerSecond = 0;
            }

            // Adds a very slight bias to pitch rate to correct any long-term drift in the pitch position, since we are commanding rates,
            // not positions.
            double simPitch = simulatorState.pitchCounts * pitchCountsToDegrees;
            double adjustedPitchRate;
            if (pitch >= 0)
            {
                adjustedPitchRate = (pitchRate + pitchAndRollDriftCorrectFactor * (Math.Min(pitch, pitchMaxCommandedDegrees) - simPitch)) * pitchDegreesToCounts * pitchScale;
            }
            else
            {
                adjustedPitchRate = (pitchRate + pitchAndRollDriftCorrectFactor * (Math.Max(pitch, -pitchMaxCommandedDegrees) - simPitch)) * pitchDegreesToCounts * pitchScale;
            }

            // As we approach physical motion limits, alter allowed pitch rates to prevent crashing.
            if (simPitch >= 0)
            {
                // If at or beyond max supported pitch, only allow negative pitch rates.
                if (pitchMaxCommandedDegrees <= simPitch)
                {
                    adjustedPitchRate = Math.Min(adjustedPitchRate, 0);
                }
                // If getting close, ramp the pitch rate down to zero as we approach the max pitch.
                else if (pitchMaxCommandedDegrees - simPitch < 10)
                {
                    adjustedPitchRate = Math.Min(adjustedPitchRate, pitchMaxCommandedDegreesPerSecond * (pitchMaxCommandedDegrees - simPitch) / 10);
                }
            }
            else
            {
                // If at or beyond min supported pitch, only allow positive pitch rates.
                if (simPitch <= -pitchMaxCommandedDegrees)
                {
                    adjustedPitchRate = Math.Max(adjustedPitchRate, 0);
                }
                // If getting close, ramp the pitch rate down to zero as we approach the min pitch.
                else if (pitchMaxCommandedDegrees + simPitch < 10)
                {
                    adjustedPitchRate = Math.Max(adjustedPitchRate, -pitchMaxCommandedDegreesPerSecond * (simPitch + pitchMaxCommandedDegrees) / 10);
                }
            }

            // Now threshold against min and max commanded pitch rates for normal operation.
            if (adjustedPitchRate >= 0)
            {
                commandedPitchRateCountsPerSecond = (int)Math.Min(adjustedPitchRate, pitchMaxCommandedCountsPerSecond);
            }
            else
            {
                commandedPitchRateCountsPerSecond = (int)Math.Max(adjustedPitchRate, -pitchMaxCommandedCountsPerSecond);
            }

            if (pitch * pitchScale >= pitchMaxCommandedDegrees || pitch * pitchScale <= -pitchMaxCommandedDegrees)
            {
                commandedPitchRateCountsPerSecond = 0;
            }

            yawCmdLabel.Text = commandedYawCounts.ToString("F1");
            pitchCmdLabel.Text = commandedPitchCounts.ToString("F1");
            rollCmdLabel.Text = commandedRollCounts.ToString("F1");

            // Converting to RPM for display
            yawRateCmdLabel.Text = (commandedYawRateCountsPerSecond / 32000 * 60).ToString("F1");
            pitchRateCmdLabel.Text = (commandedPitchRateCountsPerSecond / 32000 * 60).ToString("F1");
            rollRateCmdLabel.Text = (commandedRollRateCountsPerSecond / 32000 * 60).ToString("F1");

            if (_motorInterface != null && simulatorState != null && simulatorState.portConnected && simulatorState.motorsEnabled)
            {
                if (telemetryMotionEnabled)
                {
                    SimulatorCommand cmd = new SimulatorCommand { Name = "StartUnmonitoredMove" };
                    cmd.Data.yawRateCountsPerSecond = commandedYawRateCountsPerSecond;
                    cmd.Data.pitchRateCountsPerSecond = commandedPitchRateCountsPerSecond;
                    cmd.Data.rollRateCountsPerSecond = commandedRollRateCountsPerSecond;
                    cmd.Data.yawPositionCounts = commandedYawCounts;
                    cmd.Data.pitchPositionCounts = commandedPitchCounts;
                    cmd.Data.rollPositionCounts = commandedRollCounts;

                    cmd.Data.isVelocityCommand = true;

                    _motorInterface.EnqueueCommand(cmd);
                }

            }
        }

        private void estopButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new SimulatorCommand { Name = "DisableMotors" };
            _motorInterface?.EnqueueCommand(cmd);
            // TODO: Figure out how to interrupt anything the motors are currently doing in a blocking
            // operation. Eg. homing, moving to zero, etc.
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _receiver.Stop();
            _motorInterface?.Dispose();
            base.OnFormClosing(e);
        }

        private void MainView_Load(object sender, EventArgs e)
        {

        }

        private void telemetryZeroYawButton_Click(object sender, EventArgs e)
        {

        }

        private void simYawZeroButton_Click(object sender, EventArgs e)
        {

        }

        private void simGoToZeroButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new SimulatorCommand { Name = "GotoZero" };
            _motorInterface?.EnqueueCommand(cmd);
        }

        private void simStartStopHomingButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new SimulatorCommand { Name = "ZeroAllMotors" };
            _motorInterface?.EnqueueCommand(cmd);
        }

        private void enableTelemetryLinkButton_Click(object sender, EventArgs e)
        {
            // If currently enabled, stop motion and disable motors.
            if (telemetryMotionEnabled)
            {
                StopSimulationMotion();
            }
            // If not enabled, configure rate limits and enable motors, but only if we have an active telemetry stream.
            else if (telemetryStreamActive)
            {
                StartSimulationMotion();
            }
        }

        private void StartSimulationMotion()
        {
            if (_motorInterface != null && simulatorState != null && simulatorState.portConnected)
            {
                // Make sure rate limits are configured correctly (required, because the homing and "go to zero"
                // commands lower the rates.
                SimulatorCommand cmd = new SimulatorCommand { Name = "ConfigureRateLimits" };
                _motorInterface.EnqueueCommand(cmd);

                SimulatorCommand cmd2 = new SimulatorCommand { Name = "EnableMotors" };
                _motorInterface.EnqueueCommand(cmd2);

                telemetryMotionEnabled = true;
            }

            UpdateButtonStates();
        }

        private void StopSimulationMotion()
        {
            telemetryMotionEnabled = false;

            if (_motorInterface != null && simulatorState != null && simulatorState.portConnected)
            {
                // Stop any current motion
                SimulatorCommand cmd = new SimulatorCommand { Name = "StartUnmonitoredMove" };
                cmd.Data.yawRateCountsPerSecond = 0;
                cmd.Data.pitchRateCountsPerSecond = 0;
                cmd.Data.rollRateCountsPerSecond = 0;
                cmd.Data.isVelocityCommand = true;

                _motorInterface.EnqueueCommand(cmd);

           //     SimulatorCommand cmd2 = new SimulatorCommand { Name = "DisableMotors" };
           //     _motorInterface.EnqueueCommand(cmd2);
            }

            UpdateButtonStates();
        }

        private void trackBarYawScale_ValueChanged(object sender, EventArgs e)
        {
            if (!telemetryMotionEnabled)
            {
                yawScale = (double)trackBarYawScale.Value / 100;
                yawScaleLabel.Text = yawScale.ToString("F2");
            }
        }

        private void trackBarPitchScale_ValueChanged(object sender, EventArgs e)
        {
            if (!telemetryMotionEnabled)
            {
                pitchScale = (double)trackBarPitchScale.Value / 100;
                pitchScaleLabel.Text = pitchScale.ToString("F2");
            }
        }

        private void trackBarRollScale_ValueChanged(object sender, EventArgs e)
        {
            if (!telemetryMotionEnabled)
            {
                rollScale = (double)trackBarRollScale.Value / 100;
                rollScaleLabel.Text = rollScale.ToString("F2");
            }
        }
    }
}
