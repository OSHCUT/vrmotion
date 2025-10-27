using System.Threading.Tasks;
using TcpText;

namespace SimulatorRemoteControl
{
    public partial class SimulatorRemote : Form
    {
        private System.Windows.Forms.Timer _keepaliveTimer;

        private TcpTextClient? _client;
        private bool _isConnected = false;
        private bool _isRunning = false;
        private bool _axesZeroed = false;
        private bool _isHoming = false;
        private bool _isAtHome = false;

        public SimulatorRemote()
        {
            InitializeComponent();
            UpdatUiState();

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

            UpdatUiState();
        }

        private void MessageReceived(string line)
        {
            labelLastData.Text = line;
        }

        private void UpdatUiState()
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

            if (_isConnected)
            {
                buttonConnect.Text = "Disconnect";
                remoteConnectionStatusLabel.Text = "Connected to simulator";

                buttonStartStop.Enabled = true;
                buttonZeroAxes.Enabled = true;
                buttonGoHome.Enabled = true;
            } else
            {
                buttonConnect.Text = "Connect";
                remoteConnectionStatusLabel.Text = "Not connected";
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

            if (_isRunning)
            {
                buttonStartStop.Text = "Start";
                _isRunning = false;

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
                buttonStartStop.Text = "Stop";
                _isRunning = true;

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
