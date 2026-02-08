using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace Signal.Bot.IntegrationTests.Utils;

public class WebSocketTestServer : IAsyncDisposable
{
    private readonly HttpListener _httpListener;
    private readonly CancellationTokenSource _cts;
    private Task? _listenerTask;
    private System.Net.WebSockets.WebSocket? _serverWebSocket;

    public int Port { get; private set; }
    public string Url => $"http://localhost:{Port}/";
    public string WebSocketUrl => $"ws://localhost:{Port}/";

    public event Func<string, Task>? OnMessageReceived;
    public event Func<Task>? OnClientConnected;
    public event Func<Task>? OnClientDisconnected;

    public WebSocketTestServer(int? port = null)
    {
        Port = port ?? 0;
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(Url);
        _cts = new CancellationTokenSource();
    }

    public async Task StartAsync()
    {
        const int maxRetries = 10;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                if (Port == 0 || i > 0)
                {
                    Port = GetAvailablePort();
                }

                _httpListener.Prefixes.Clear();
                _httpListener.Prefixes.Add(Url);
                _httpListener.Start();
                _listenerTask = Task.Run(() => ListenAsync(_cts.Token));
                await Task.Delay(100);
                return;
            }
            catch (HttpListenerException) when (i < maxRetries - 1)
            {
                await Task.Delay(100);
            }
        }

        if (!_httpListener.IsListening)
        {
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add(Url);
            _httpListener.Start();
            _listenerTask = Task.Run(() => ListenAsync(_cts.Token));
            await Task.Delay(100);
        }
    }


    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var context = await _httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    _serverWebSocket = wsContext.WebSocket;

                    if (OnClientConnected != null)
                    {
                        await OnClientConnected.Invoke();
                    }

                    _ = Task.Run(async () => await HandleWebSocketAsync(_serverWebSocket, cancellationToken),
                        cancellationToken);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected when stopping
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }
    }

    private async Task HandleWebSocketAsync(System.Net.WebSockets.WebSocket webSocket,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        cancellationToken);

                    if (OnClientDisconnected != null)
                    {
                        await OnClientDisconnected.Invoke();
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (OnMessageReceived != null)
                    {
                        await OnMessageReceived.Invoke(message);
                    }
                }
            }
        }
        catch (WebSocketException)
        {
            if (OnClientDisconnected != null)
            {
                await OnClientDisconnected.Invoke();
            }
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_serverWebSocket?.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _serverWebSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    public async Task SendBinaryMessageAsync(byte[] data)
    {
        if (_serverWebSocket?.State == WebSocketState.Open)
        {
            await _serverWebSocket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Binary,
                true,
                CancellationToken.None);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_serverWebSocket?.State == WebSocketState.Open)
        {
            await _serverWebSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Server disconnect",
                CancellationToken.None);
        }
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        _serverWebSocket?.Dispose();
        _httpListener.Stop();
        _httpListener.Close();
        _cts.Dispose();
        await (_listenerTask?.WaitAsync(TimeSpan.FromSeconds(2)) ?? Task.CompletedTask);
        GC.SuppressFinalize(this);
    }
}
