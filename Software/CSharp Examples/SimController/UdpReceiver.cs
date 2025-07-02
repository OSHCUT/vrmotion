using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace SimController
{
    class UdpReceiver
    {
        private readonly int _port;
        private readonly UdpClient _udpClient;
        private readonly CancellationTokenSource _cts = new();

        public event Action<string>? MessageReceived;

        public UdpReceiver(int port)
        {
            _port = port;
            _udpClient = new UdpClient(_port);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var result = await _udpClient.ReceiveAsync();
                        string message = Encoding.UTF8.GetString(result.Buffer);

                        MessageReceived?.Invoke(message); // Raise event
                    }
                }
                catch (ObjectDisposedException) { }
            });
        }

        public void Stop()
        {
            _cts.Cancel();
            _udpClient.Close();
        }
    }
}