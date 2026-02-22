using Signal.Bot.Types;

namespace Signal.Bot.Example.DocuCode;

public static class EchoBotProgramm
{
    #region EchoBot
    public static void Main(string[] args)
    {
        // Configuration
        const string apiUrl = "http://localhost:8080";
        const string botNumber = "+1234567890"; // Replace with your Signal number

        // Create client
        var client = new SignalBotClient(x => x.WithBaseUrl(apiUrl).WithNumber(botNumber));

        // Cancellation token for graceful shutdown
        using var cts = new CancellationTokenSource();

        // Handle Ctrl+C
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nStopping bot...");
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("Echo Bot is starting...");
        Console.WriteLine($"Bot number: {botNumber}");
        Console.WriteLine("Press Ctrl+C to stop\n");

        // Start receiving messages
        client.StartReceiving(
            updateHandler: HandleMessage,
            errorHandler: HandleError,
            cancellationToken:
            cts.Token
        );

        Console.WriteLine("Bot stopped");
    }

    public static async Task HandleMessage(ISignalBotClient client, ReceivedMessage message, CancellationToken token)
    {
        var envelope = message.Envelope!;
        // Get the text message
        var text = envelope.DataMessage?.Message;

        // Ignore empty messages
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        // Get sender
        var sender = message.Envelope?.SourceNumber ?? string.Empty;

        // Log received message
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] From {sender}: {text}");

        // Echo back with a prefix
        var echoMessage = $"You said: {text}";

        try
        {
            // Send the echo
            await client.SendMessageAsync(builder => builder.WithMessage(echoMessage).WithRecipient(sender),
                cancellationToken: token);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Echoed back to {sender}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending echo: {ex.Message}");
        }
    }

    public static async Task HandleError(ISignalBotClient client, Error err, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ERROR] {err.Exception?.GetType().Name}: {err.Exception?.Message}");

        // Wait a bit before continuing to avoid tight error loops
        await Task.Delay(1000, cancellationToken);
    }
    #endregion EchoBot
}