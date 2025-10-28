using System.Threading.Tasks;
using TcpText;

namespace SimulatorRemoteControl
{
    public partial class SimulatorRemote : Form
    {
        private System.Windows.Forms.Timer _keepaliveTimer;

        private TcpTextClient? _client;
        private bool _isConnected = false;
        private bool _isReadyToMove = false;
        private bool _isTelemetryStreamActive = false;
        private bool _isTelemetryMotionEnabled = false;
        private bool _isHoming = false;
        private bool _isHomed = false;
        private bool _isMovingToZero = false;

        public SimulatorRemote()
        {
            InitializeComponent();
            UpdateUiState();

            _keepaliveTimer = new System.Windows.Forms.Timer();
            _keepaliveTimer.Interval = 200; // milliseconds => 5 Hz
            _keepaliveTimer.Tick += KeepaliveTimer_Tick;
            _keepaliveTimer.Start();
        }

        private async void KeepaliveTimer_Tick(object? sender, EventArgs e)
        {
            if (_isConnected && _client != null && _client.IsConnected)
            {
                try
                {
                    await _client.SendAsync("KEEPALIVE");
                }
                catch (Exception ex)
                {
                    remoteConnectionStatusLabel.Text = "ERROR: Failed to send keepalive. Disconnecting.";
                    DisconnectClient();
                }                
            }
        }

        private void DisconnectClient()
        {
            if (_client != null)
            {
                _client.Dispose();
                _isConnected = false;
            }
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (_client != null && _client.IsConnected)
            {
                DisconnectClient();
            }
            else
            {
                remoteConnectionStatusLabel.Text = "Connecting...";

                _client?.Dispose();
                _client = new TcpTextClient(this);
                _client.StatusChanged += (_, msg) => ConnectionStatusChanged(msg);
                _client.MessageReceived += (_, line) => MessageReceived(line);

                try
                {
                    await _client.ConnectAsync("127.0.0.1", 5555);
                    remoteConnectionStatusLabel.Text = "Connected.";
                } catch (Exception ex)
                {
                    remoteConnectionStatusLabel.Text = "Failed to connect. Try again.";
                    return;
                }
            }
        }

        private void ConnectionStatusChanged(string status)
        {
            if (status.StartsWith("Connected to"))
            {
                _isConnected = true;
            }
            else if (status.StartsWith("Client disconnected"))
            {
                _isConnected = false;
            }

            UpdateUiState();
        }

        private void MessageReceived(string line)
        {
            labelLastData.Text = line;

            if (line.IndexOf("readyToMove\": true") >= 0)
            {
                _isReadyToMove = true;
            } else
            {
                _isReadyToMove = false;
            }

            if (line.IndexOf("telemetryStreamActive\": true") >= 0)
            {
                _isTelemetryStreamActive = true;
            }
            else
            {
                _isTelemetryStreamActive = false;
            }

            if (line.IndexOf("telemetryMotionEnabled\": true") >= 0)
            {
                _isTelemetryMotionEnabled = true;
            }
            else
            {
                _isTelemetryMotionEnabled = false;
            }

            if (line.IndexOf("isHoming\": true") >= 0)
            {
                _isHoming = true;
            }
            else
            {
                _isHoming = false;
            }

            if (line.IndexOf("isHomed\": true") >= 0)
            {
                _isHomed = true;
            }
            else
            {
                _isHomed = false;
            }

            if (line.IndexOf("isMovingToZero\": true") >= 0)
            {
                _isMovingToZero = true;
            }
            else
            {
                _isMovingToZero = false;
            }

            UpdateUiState();
        }

        private void UpdateUiState()
        {
            if (_client == null || !_client.IsConnected)
            {
                buttonStartStop.Enabled = false;
                buttonZeroAxes.Enabled = false;
                buttonGoHome.Enabled = false;

                buttonConnect.Text = "Connect";
                remoteConnectionStatusLabel.Text = "Not connected";

                return;
            }

            // Start/stop button enabled if the machine is ready to move, it is homed, and telemetry stream is active
            if (_isTelemetryMotionEnabled)
            {
                buttonStartStop.Enabled = true;
                buttonStartStop.Text = "Stop";
            }
            // If telemetry motion is enabled, we can always stop
            else
            {
                buttonStartStop.Text = "Start";

                if (_isReadyToMove && _isTelemetryStreamActive && _isHomed)
                {
                    buttonStartStop.Enabled = true;
                }
                else
                {
                    buttonStartStop.Enabled = false;
                }
            }

            // Go home button
            if (!_isTelemetryMotionEnabled && _isHomed && _isReadyToMove && !_isMovingToZero)
            {
                buttonGoHome.Enabled = true;
            }
            else
            {
                buttonGoHome.Enabled = false;
            }

            if (_isMovingToZero)
            {
                buttonGoHome.Text = "Moving Home...";
            } else
            {
                buttonGoHome.Text = "Go Home";
            }

            // Zero axes button
            if (!_isTelemetryMotionEnabled && _isReadyToMove && !_isMovingToZero && !_isHoming)
            {
                buttonZeroAxes.Enabled = true;
            }
            else
            {
                buttonZeroAxes.Enabled = false;
            }
        }

        private async void buttonZeroAxes_Click(object sender, EventArgs e)
        {
            // TODO: Check existing state and make sure we can do this
            if (_client == null || !_client.IsConnected)
                return;
            
            try
            {
                await _client.SendAsync("ZERO_AXES");
            } catch (Exception ex)
            {
                remoteConnectionStatusLabel.Text = "ERROR: Failed to send zero axes command. Disconnecting.";
                DisconnectClient();
            }
            
        }

        private async void buttonStartStop_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected)
                return;

            if (_isTelemetryMotionEnabled)
            {
                try
                {
                    await _client.SendAsync("STOP_SIMULATION");
                }
                catch (Exception ex)
                {
                    remoteConnectionStatusLabel.Text = "ERROR: Failed to send stop command. Disconnecting.";
                    DisconnectClient();
                }
                
            }
            else
            {
                try
                {
                    await _client.SendAsync("START_SIMULATION");
                }
                catch (Exception ex)
                {
                    remoteConnectionStatusLabel.Text = "ERROR: Failed to send start command. Disconnecting.";
                    DisconnectClient();
                }
            }

            UpdateUiState();
        }

        private async void buttonGoHome_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected)
                return;

            try
            {
                await _client.SendAsync("GO_HOME");
            }
            catch (Exception ex)
            {
                remoteConnectionStatusLabel.Text = "ERROR: Failed to send go home command. Disconnecting.";
                DisconnectClient();
            }
        }
    }
}
