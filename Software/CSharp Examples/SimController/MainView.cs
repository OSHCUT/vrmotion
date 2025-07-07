using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimController
{
    public partial class MainView : Form
    {
        private readonly UdpReceiver _receiver;
        private MotorInterface? _motorInterface;
        private System.Windows.Forms.Timer _pollTimer;


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
        private const double rollMaxCommandedDegrees = 40;
        private const double rollMaxCommandedDegreesPerSecond = 90;
        private const double pitchMaxCommandedDegreesPerSecond = 90;
        private const double yawMaxCommandedDegreesPerSecond = 90;
        private const long pitchMaxCommandedCounts = (long)(pitchMaxCommandedDegrees * pitchDegreesToCounts);
        private const long rollMaxCommandedCounts = (long)(rollMaxCommandedDegrees * rollDegreesToCounts);
        private const long yawMaxCommandedCountsPerSecond = (long)(yawMaxCommandedDegreesPerSecond * yawDegreesToCounts);
        private const long rollMaxCommandedCountsPerSecond = (long)(rollMaxCommandedDegreesPerSecond * rollDegreesToCounts);
        private const long pitchMaxCommandedCountsPerSecond = (long)(pitchMaxCommandedDegreesPerSecond * pitchDegreesToCounts);

        private long yawZeroCounts = 0;
        private double yawRateScale = 0;  // Affects how accurately yaw rate matches telemetry data. Should range from 0.1 to 1.0.
        private double rollPositionScale = 1; // Scale for roll position, affects how exactly roll matches telemetry data. Should range from 0.1 to 1.0. Numbers < 1.0 increase simulation "dynamic range"
        private double pitchPositionScale = 1; // Scale for pitch position, affects how exactly pitch matches telemetry data. Should range from 0.1 to 1.0.

        private int commandedYawCounts = 0;
        private double commandedYawRateCountsPerSecond = 0;

        private int commandedRollCounts = 0;
        private double commandedRollRateCountsPerSecond = 0;

        private int commandedPitchCounts = 0;
        private double commandedPitchRateCountsPerSecond = 0;

        private bool telemetryMotionEnabled = false; // If true, telemetry data is used to control motors.
        private SimulatorState? simulatorState;

        const double pitchAndRollDriftCorrectFactor = 0.4; // A small bias to correct for drift in pitch and roll positions over time.

        public MainView()
        {
            InitializeComponent();

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
        }

        private void PollTimer_Tick(object? sender, EventArgs e)
        {
            if (simulatorState != null && simulatorState.portConnected)
            {
                SimulatorCommand pollCmd = new SimulatorCommand { Name = "GetState" };
                _motorInterface?.EnqueueCommand(pollCmd);
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
            if (simulatorState.portConnected)
            {
                simEnableDisableButton.Enabled = true;
            }
            else
            {
                simEnableDisableButton.Enabled = false;
            }

            if (simulatorState.motorsEnabled)
            {
                simEnableDisableButton.Text = "Disable";

                if (simulatorState.homingInProgress || simulatorState.movingToZero)
                {
                    simStartStopHomingButton.Enabled = false;
                    simGoToZeroButton.Enabled = false;
                    enableTelemetryLinkButton.Enabled = true;
                }
                else
                {
                    simStartStopHomingButton.Enabled = true;

                    if (simulatorState.motorsHomed)
                    {
                        simGoToZeroButton.Enabled = true;
                        enableTelemetryLinkButton.Enabled = true;
                    }
                    enableTelemetryLinkButton.Enabled = true;
                }
            }
            else
            {
                simEnableDisableButton.Text = "Enable";

                simGoToZeroButton.Enabled = false;
                simStartStopHomingButton.Enabled = false;
                simYawZeroButton.Enabled = false;
                enableTelemetryLinkButton.Enabled = false;
            }

            // Update motor state values
            simYawLabel.Text = simulatorState.yawRate.ToString("F1");
            simPitchLabel.Text = simulatorState.pitchCounts.ToString("F1");
            simRollLabel.Text = simulatorState.rollCounts.ToString("F1");
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
                commandedPitchCounts = (int)Math.Min(pitch * pitchDegreesToCounts * pitchPositionScale, pitchMaxCommandedCounts);
            }
            else
            {
                commandedPitchCounts = (int)Math.Max(pitch * pitchDegreesToCounts * pitchPositionScale, -pitchMaxCommandedCounts);
            }

            if (roll >= 0)
            {
                commandedRollCounts = (int)Math.Min(roll * rollDegreesToCounts * rollPositionScale, rollMaxCommandedCounts);
            }
            else
            {
                commandedRollCounts = (int)Math.Max(roll * rollDegreesToCounts * rollPositionScale, -rollMaxCommandedCounts);
            }

            // Rates
            if (yawRate >= 0)
            {
                commandedYawRateCountsPerSecond = Math.Min(yawRate * yawDegreesToCounts * yawRateScale, yawMaxCommandedCountsPerSecond);
            }
            else
            {
                commandedYawRateCountsPerSecond = Math.Max(yawRate * yawDegreesToCounts * yawRateScale, -yawMaxCommandedCountsPerSecond);
            }

            // Adds a very slight bias to roll rate to correct any long-term drift in the roll position, since we are commanding rates,
            // not positions.
            double simRoll = simulatorState.rollCounts * rollCountsToDegrees;
            double adjustedRollRate;
            if (roll >= 0)
            {
                adjustedRollRate = (rollRate + pitchAndRollDriftCorrectFactor * (Math.Min(roll, rollMaxCommandedDegrees) - simRoll)) * rollDegreesToCounts * rollPositionScale;
            }
            else
            {
                adjustedRollRate = (rollRate + pitchAndRollDriftCorrectFactor * (Math.Max(roll, -rollMaxCommandedDegrees) - simRoll)) * rollDegreesToCounts * rollPositionScale;
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
            if (roll >= rollMaxCommandedDegrees || roll <= -rollMaxCommandedDegrees)
            {
                commandedRollRateCountsPerSecond = 0;
            }

            // Adds a very slight bias to pitch rate to correct any long-term drift in the pitch position, since we are commanding rates,
            // not positions.
            double simPitch = simulatorState.pitchCounts * pitchCountsToDegrees;
            double adjustedPitchRate;
            if (pitch >= 0)
            {
                adjustedPitchRate = (pitchRate + pitchAndRollDriftCorrectFactor * (Math.Min(pitch, pitchMaxCommandedDegrees) - simPitch)) * pitchDegreesToCounts * pitchPositionScale;
            } else
            {
                adjustedPitchRate = (pitchRate + pitchAndRollDriftCorrectFactor * (Math.Max(pitch, -pitchMaxCommandedDegrees) - simPitch)) * pitchDegreesToCounts * pitchPositionScale;
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

            if (pitch >= pitchMaxCommandedDegrees || pitch <= -pitchMaxCommandedDegrees)
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

            if (_motorInterface != null && simulatorState != null && simulatorState.portConnected)
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

        private void simEnableDisableButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new();
            if (simulatorState == null || simulatorState.motorsEnabled == false)
            {
                cmd.Name = "EnableMotors";
                _motorInterface?.EnqueueCommand(cmd);
            }
            else
            {
                cmd.Name = "DisableMotors";
                _motorInterface?.EnqueueCommand(cmd);
            }
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
            if (telemetryMotionEnabled)
            {
                telemetryMotionEnabled = false;
                enableTelemetryLinkButton.Text = "Enable Motion";

                if (_motorInterface != null && simulatorState != null && simulatorState.portConnected)
                {
                    // Stop any current motion
                    SimulatorCommand cmd = new SimulatorCommand { Name = "StartUnmonitoredMove" };
                    cmd.Data.yawRateCountsPerSecond = 0;
                    cmd.Data.pitchRateCountsPerSecond = 0;
                    cmd.Data.rollRateCountsPerSecond = 0;
                    cmd.Data.isVelocityCommand = true;

                    _motorInterface.EnqueueCommand(cmd);
                }                
            }
            else
            {
                telemetryMotionEnabled = true;
                enableTelemetryLinkButton.Text = "Disable Motion";
            }
        }

        private void testMoveButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new SimulatorCommand { Name = "StartUnmonitoredMove" };
            cmd.Data.yawRateCountsPerSecond = 0;
            cmd.Data.yawPositionCounts = 0;
            cmd.Data.pitchPositionCounts = (int)(10 * pitchDegreesToCounts);
            cmd.Data.rollPositionCounts = (int)(10 * rollDegreesToCounts);
            cmd.Data.isVelocityCommand = false;
            _motorInterface?.EnqueueCommand(cmd);
        }
    }
}
