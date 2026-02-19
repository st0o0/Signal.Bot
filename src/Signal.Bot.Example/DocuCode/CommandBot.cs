using Signal.Bot.Types;

namespace Signal.Bot.Example.DocuCode;

public static class CommandBot
{
    #region CommandBot
    private static SignalBotClient _client;
    private static string _botNumber = string.Empty;

    public static void Main(string[] args)
    {
        const string apiUrl = "http://localhost:8080";
        _botNumber = "+1234567890";

        _client = new SignalBotClient(builder => builder.WithBaseUrl(apiUrl).WithNumber(_botNumber));

        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nStopping bot...");
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("Command Bot is running...");
        Console.WriteLine("Available commands: /help, /time, /echo, /ping, /info");
        Console.WriteLine("Press Ctrl+C to stop\n");

        _client.StartReceiving(updateHandler: HandleMessage, errorHandler: HandleError, cancellationToken: cts.Token);
    }

    public static async Task HandleMessage(ISignalBotClient client, ReceivedMessageEnvelope message,
        CancellationToken cancellationToken)
    {
        var envelope = message.Envelope!;
        var text = envelope.DataMessage?.Message;
        if (string.IsNullOrWhiteSpace(text)) return;

        var sender = envelope.SourceNumber!;

        // Only process commands (messages starting with /)
        if (!text.StartsWith('/'))
        {
            // Optionally send a hint
            await SendMessage(sender, "Send /help to see available commands", cancellationToken);
            return;
        }

        // Parse command and arguments
        var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1] : string.Empty;

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Command from {sender}: {command}");

        // Route to appropriate handler
        await (command switch
        {
            "/start" => HandleStart(sender, cancellationToken),
            "/help" => HandleHelp(sender, cancellationToken),
            "/time" => HandleTime(sender, cancellationToken),
            "/date" => HandleDate(sender, cancellationToken),
            "/echo" => HandleEcho(sender, args, cancellationToken),
            "/ping" => HandlePing(sender, cancellationToken),
            "/info" => HandleInfo(sender, cancellationToken),
            "/uptime" => HandleUptime(sender, cancellationToken),
            _ => HandleUnknownCommand(sender, command, cancellationToken)
        });
    }

    public static async Task HandleStart(string sender, CancellationToken ct)
    {
        const string message = """
                               👋 Welcome to Command Bot!

                               I'm a bot that responds to commands. Send /help to see what I can do!
                               """;

        await SendMessage(sender, message, ct);
    }

    public static async Task HandleHelp(string sender, CancellationToken ct)
    {
        const string message = """
                               🤖 Available Commands

                               📍 Basic:
                               /start - Welcome message
                               /help - Show this message
                               /ping - Check if bot is alive

                               ⏰ Time & Date:
                               /time - Get current time
                               /date - Get current date
                               /uptime - Bot uptime

                               💬 Utilities:
                               /echo <text> - Echo back your message
                               /info - Bot information
                               """;

        await SendMessage(sender, message, ct);
    }

    public static async Task HandleTime(string sender, CancellationToken ct)
    {
        var message = $"🕐 Current time: {DateTime.Now:HH:mm:ss}";
        await SendMessage(sender, message, ct);
    }

    public static async Task HandleDate(string sender, CancellationToken ct)
    {
        var message = $"📅 Today is: {DateTime.Now:dddd, MMMM dd, yyyy}";
        await SendMessage(sender, message, ct);
    }

    public static async Task HandleEcho(string sender, string args, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            await SendMessage(sender, "❌ Usage: /echo <text>", ct);
            return;
        }

        await SendMessage(sender, args, ct);
    }

    public static async Task HandlePing(string sender, CancellationToken ct)
    {
        var startTime = DateTime.UtcNow;

        await SendMessage(sender, "🏓 Pong!", ct);

        var latency = (DateTime.UtcNow - startTime).TotalMilliseconds;
        Console.WriteLine($"Ping latency: {latency:F0}ms");
    }

    public static async Task HandleInfo(string sender, CancellationToken ct)
    {
        var message = $"""
                       ℹ️ Bot Information

                       📱 Number: {_botNumber}
                       🤖 Framework: Signal.Bot
                       ⚙️ Runtime: .NET {Environment.Version}
                       💻 Platform: {Environment.OSVersion}
                       📊 Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB
                       """;

        await SendMessage(sender, message, ct);
    }

    private static readonly DateTime _startTime = DateTime.Now;

    public static async Task HandleUptime(string sender, CancellationToken ct)
    {
        var uptime = DateTime.Now - _startTime;
        var message = $"⏱ Bot uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";

        await SendMessage(sender, message, ct);
    }

    public static async Task HandleUnknownCommand(string sender, string command, CancellationToken ct)
    {
        var message = $"❓ Unknown command: {command}\n\nSend /help to see available commands.";
        await SendMessage(sender, message, ct);
    }

    public static async Task SendMessage(string recipient, string message, CancellationToken ct)
    {
        try
        {
            await _client.SendMessageAsync(builder => builder.WithMessage(message).WithRecipient(recipient), ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public static async Task HandleError(
        ISignalBotClient client,
        Error error,
        CancellationToken ct)
    {
        Console.WriteLine($"[ERROR] {error.Source}");
        await Task.Delay(1000, ct);
    }
    #endregion CommandBot
}