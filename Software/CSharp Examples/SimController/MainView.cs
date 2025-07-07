using System.Globalization;

namespace SimController
{
    public partial class MainView : Form
    {
        private readonly UdpReceiver _receiver;
        private MotorInterface? _motorInterface;

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
        private const double yawMaxCommandedDegreesPerSecond = 90;
        private const long pitchMaxCommandedCounts = (long)(pitchMaxCommandedDegrees * pitchDegreesToCounts);
        private const long rollMaxCommandedCounts = (long)(rollMaxCommandedDegrees * rollDegreesToCounts);
        private const long yawMaxCommandedCountsPerSecond = (long)(yawMaxCommandedDegreesPerSecond * yawDegreesToCounts);

        private long yawZeroCounts = 0;
        private Boolean yawRateMode = true;    // If true, yaw is moved by telemetry rate, not position.
        private double yawRateScale = 0.1;  // Affects how accurately yaw rate matches telemetry data. Should range from 0.1 to 1.0.
        private double rollPositionScale = 1.0; // Scale for roll position, affects how exactly roll matches telemetry data. Should range from 0.1 to 1.0. Numbers < 1.0 increase simulation "dynamic range"
        private double pitchPositionScale = 1.0; // Scale for pitch position, affects how exactly pitch matches telemetry data. Should range from 0.1 to 1.0.

        private int commandedYawCounts = 0;
        private double commandedYawRateCountsPerSecond = 0;
        private int commandedRollCounts = 0;
        private int commandedPitchCounts = 0;

        private Boolean telemetryMotionEnabled = false; // If true, telemetry data is used to control motors.
        private SimulatorState? simulatorState;

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
            simYawLabel.Text = simulatorState.yawRate.ToString();
            simPitchLabel.Text = simulatorState.pitchCounts.ToString();
            simRollLabel.Text = simulatorState.rollCounts.ToString();

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
            // I've set SimTools up to output 16 bit unsigned integers, which gives a range of 0 to 65535.
            // I've also set up the min/max value ranges from -180 to 180 degrees for yaw, roll, and pitch,
            // and -10 to 10 for heave, sway, and surge, and -90 to 90 degrees per second for yaw, roll, and pitch rates.
            // This yields the conversions below.
            yaw = (yawRaw / 65535.0) * 360.0 - 180.0;
            roll = (rollRaw / 65535.0) * 360.0 - 180.0;
            pitch = (pitchRaw / 65535.0) * 360.0 - 180.0;
            heave = (heaveRaw / 65535.0) * 20.0 - 10.0;
            sway = (swayRaw / 65535.0) * 20.0 - 10.0;
            surge = (surgeRaw / 65535.0) * 20.0 - 10.0;
            yawRate = (yawRateRaw / 65535.0) * 180 - 90.0;
            rollRate = (rollRateRaw / 65535.0) * 180 - 90.0;
            pitchRate = (pitchRateRaw / 65535.0) * 180 - 90.0;

            telemetryYawLabel.Text = $"{yaw:F2}°";
            telemetryRollLabel.Text = $"{roll:F2}°";
            telemetryPitchLabel.Text = $"{pitch:F2}°";
            telemetryHeaveLabel.Text = $"{heave:F2}";
            telemetrySwayLabel.Text = $"{sway:F2}";
            telemetrySurgeLabel.Text = $"{surge:F2}";
            telemetryYawRateLabel.Text = $"{yawRate:F2}°/s";
            telemetryRollRateLabel.Text = $"{rollRate:F2}°/s";
            telemetryPitchRateLabel.Text = $"{pitchRate:F2}°/s";

            if (yawRate >= 0)
            {
                commandedYawRateCountsPerSecond = Math.Min(yawRate * yawDegreesToCounts * yawRateScale, yawMaxCommandedCountsPerSecond);
            }
            else
            {
                commandedYawRateCountsPerSecond = Math.Max(yawRate * yawDegreesToCounts * yawRateScale, -yawMaxCommandedCountsPerSecond);
            }
            commandedYawCounts = (int)(yaw * yawDegreesToCounts - yawZeroCounts);  // TODO: Deal with rollover at 180 degrees

            if (roll >= 0)
            {
                commandedRollCounts = (int)Math.Min(roll * rollDegreesToCounts * rollPositionScale, rollMaxCommandedCounts);
            }
            else
            {
                commandedRollCounts = (int)Math.Max(roll * rollDegreesToCounts * rollPositionScale, -rollMaxCommandedCounts);
            }

            if (pitch >= 0)
            {
                commandedPitchCounts = (int)Math.Min(pitch * pitchDegreesToCounts * pitchPositionScale, pitchMaxCommandedCounts);
            }
            else
            {
                commandedPitchCounts = (int)Math.Max(pitch * pitchDegreesToCounts * pitchPositionScale, -pitchMaxCommandedCounts);
            }

            pitchCmdLabel.Text = commandedPitchCounts.ToString();
            rollCmdLabel.Text = commandedRollCounts.ToString();
            yawRateCmdLabel.Text = (commandedYawRateCountsPerSecond / 32000 * 60).ToString();

            if (telemetryMotionEnabled)
            {
                if (_motorInterface != null)
                {
                    SimulatorCommand cmd = new();
                    cmd.Name = "StartUnmonitoredMove";
                    cmd.Data.yawRateCountsPerSecond = commandedYawRateCountsPerSecond;
                    cmd.Data.pitchPositionCounts = commandedPitchCounts;
                    cmd.Data.rollPositionCounts = commandedRollCounts;
                    _motorInterface.EnqueueCommand(cmd);
                }
            }
        }

        private void estopButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new();
            cmd.Name = "DisableMotors";
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
            SimulatorCommand cmd = new();
            cmd.Name = "GotoZero";
            _motorInterface?.EnqueueCommand(cmd);
        }

        private void simStartStopHomingButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new();
            cmd.Name = "ZeroAllMotors";
            _motorInterface?.EnqueueCommand(cmd);
        }

        private void enableTelemetryLinkButton_Click(object sender, EventArgs e)
        {
            if (telemetryMotionEnabled)
            {
                telemetryMotionEnabled = false;
                enableTelemetryLinkButton.Text = "Enable Motion";
            }
            else
            {
                telemetryMotionEnabled = true;
                enableTelemetryLinkButton.Text = "Disable Motion";
            }
        }

        private void testMoveButton_Click(object sender, EventArgs e)
        {
            SimulatorCommand cmd = new();
            cmd.Name = "StartUnmonitoredMove";
            cmd.Data.yawRateCountsPerSecond = 0;
            cmd.Data.pitchPositionCounts = (int)(15 * pitchDegreesToCounts);
            cmd.Data.rollPositionCounts = (int)(15 * rollDegreesToCounts);
            _motorInterface?.EnqueueCommand(cmd);
        }
    }
}
