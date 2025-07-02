namespace SimController
{
    public partial class MainView : Form
    {
        private readonly UdpReceiver _receiver;
        private MotorInterface? motorInterface;

        public MainView()
        {
            InitializeComponent();

            _receiver = new UdpReceiver(5123);
            _receiver.MessageReceived += OnUdpMessageReceived;
            _receiver.Start();
        }

        private void OnUdpMessageReceived(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateLabel(message));
            }
            else
            {
                UpdateLabel(message);
            }
        }

        private void UpdateLabel(string message)
        {
            motorComStatusLabel.Text = message;
        }

        private void estopButton_Click(object sender, EventArgs e)
        {
            // TODO: Stop all motion, disable all motors!

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _receiver.Stop();
            base.OnFormClosing(e);
        }
    }
}
