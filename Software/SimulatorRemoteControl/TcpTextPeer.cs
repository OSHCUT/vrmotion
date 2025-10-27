using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpText
{
    internal static class UiMarshal
    {
        public static void OnUI(Control? ui, Action action)
        {
            if (ui == null || ui.IsDisposed) return;
            if (ui.InvokeRequired)
            {
                try { ui.BeginInvoke(action); } catch { /* ignore */ }
            }
            else
            {
                action();
            }
        }
    }

    public abstract class TcpTextBase : IDisposable
    {
        protected readonly Control? _ui; // marshal events to UI thread
        protected readonly Encoding _encoding = new UTF8Encoding(false);
        protected CancellationTokenSource? _cts;
        protected TcpClient? _socket;
        protected StreamReader? _reader;
        protected StreamWriter? _writer;
        protected readonly object _sendLock = new();

        public event EventHandler<string>? MessageReceived;   // Raised on UI thread
        public event EventHandler<string>? StatusChanged;     // Raised on UI thread
        public bool IsConnected => _socket?.Connected == true;

        protected TcpTextBase(Control? ui) => _ui = ui;

        protected void RaiseStatus(string msg)
        {
            // Fire on background, then marshal to UI
            _ = Task.Run(() => UiMarshal.OnUI(_ui, () => StatusChanged?.Invoke(this, msg)));
        }

        protected void RaiseMessage(string msg)
        {
            // Fire on background, then marshal to UI
            _ = Task.Run(() => UiMarshal.OnUI(_ui, () => MessageReceived?.Invoke(this, msg)));
        }

        protected async Task ReaderLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && _reader != null)
                {
                    var line = await _reader.ReadLineAsync().WaitAsync(ct);
                    if (line == null) break; // disconnected
                    RaiseMessage(line);
                }
            }
            catch (OperationCanceledException) { /* shutting down */ }
            catch (IOException) { /* remote closed / network issue */ }
            catch (ObjectDisposedException) { /* shutting down */ }
            finally
            {
                RaiseStatus("Disconnected.");
                await CloseSocketAsync();
            }
        }

        public async Task SendAsync(string text, CancellationToken ct = default)
        {
            if (_writer == null) throw new InvalidOperationException("Not connected.");
            lock (_sendLock)
            {
                _writer.WriteLine(text);
                _writer.Flush();
            }
            await Task.CompletedTask;
        }

        protected void AttachIo(NetworkStream ns)
        {
            _reader = new StreamReader(ns, _encoding, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
            _writer = new StreamWriter(ns, _encoding, bufferSize: 4096, leaveOpen: true) { AutoFlush = true, NewLine = "\n" };
        }

        protected async Task CloseSocketAsync()
        {
            try { _writer?.Dispose(); } catch { }
            try { _reader?.Dispose(); } catch { }
            try { _socket?.Close(); } catch { }
            _writer = null;
            _reader = null;
            _socket = null;
            await Task.CompletedTask;
        }

        public virtual async Task StopAsync()
        {
            try { _cts?.Cancel(); } catch { }
            await CloseSocketAsync();
        }

        public void Dispose()
        {
            _ = StopAsync();
            _cts?.Dispose();
        }
    }

    // ----- SERVER (single-connection) -----
    public sealed class TcpTextServer : TcpTextBase
    {
        private TcpListener? _listener;
        private readonly object _acceptLock = new();
        private bool _accepting;

        public int Port { get; private set; }

        public TcpTextServer(Control? ui) : base(ui) { }

        public async Task StartAsync(int port, IPAddress? ip = null)
        {
            await StopAsync();

            _cts = new CancellationTokenSource();
            Port = port;
            _listener = new TcpListener(ip ?? IPAddress.Any, port);
            _listener.Start();
            RaiseStatus($"Server listening on {(_listener.LocalEndpoint as IPEndPoint)!}.");

            _ = Task.Run(() => AcceptLoopAsync(_cts.Token));
        }

        private async Task AcceptLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    lock (_acceptLock)
                    {
                        if (_accepting) return; // ensure one accept loop
                        _accepting = true;
                    }

                    // Only allow one active client at a time.
                    if (IsConnected)
                    {
                        await Task.Delay(200, ct);
                        continue;
                    }

                    var client = await _listener!.AcceptTcpClientAsync(ct);
                    _socket = client;
                    var ns = client.GetStream();
                    AttachIo(ns);
                    RaiseStatus("Client connected.");

                    _ = Task.Run(() => ReaderLoopAsync(ct), ct);
                }
                catch (OperationCanceledException) { return; }
                catch (ObjectDisposedException) { return; }
                catch (Exception ex)
                {
                    RaiseStatus($"Accept error: {ex.Message}");
                    await Task.Delay(500, ct);
                }
                finally
                {
                    lock (_acceptLock) { _accepting = false; }
                }
            }
        }

        public override async Task StopAsync()
        {
            await base.StopAsync();
            try { _listener?.Stop(); } catch { }
            _listener = null;
            RaiseStatus("Server stopped.");
        }
    }

    // ----- CLIENT -----
    public sealed class TcpTextClient : TcpTextBase
    {
        public TcpTextClient(Control? ui) : base(ui) { }

        public async Task ConnectAsync(string host, int port, CancellationToken ct = default)
        {
            await StopAsync();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _socket = new TcpClient();
            await _socket.ConnectAsync(host, port, _cts.Token);
            AttachIo(_socket.GetStream());
            RaiseStatus($"Connected to {host}:{port}.");

            _ = Task.Run(() => ReaderLoopAsync(_cts.Token), _cts.Token);
        }

        public async Task DisconnectAsync()
        {
            await StopAsync();
            RaiseStatus("Client disconnected.");
        }
    }
}
