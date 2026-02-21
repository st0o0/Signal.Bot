namespace Signal.Bot.Example;

public class ReadMe
{
    public void QuickStart()
    {
        const string baseUrl = "http://localhost:8080";
        // Your registered Signal number
        const string botNumber = "+1234567890";

        var client = new SignalBotClient(builder =>
        {
            builder
                .WithBaseUrl(baseUrl)
                .WithNumber(botNumber);
        });

        using var cts = new CancellationTokenSource();

        client.StartReceiving(async (botClient, message, ct) =>
            {
                Console.WriteLine($"Received from {message.Account}: {message.Envelope?.DataMessage?.Message}");

                // Echo the message back
                if (!string.IsNullOrEmpty(message.Envelope?.DataMessage?.Message))
                {
                    await botClient.SendMessageAsync(
                        builder => builder
                            .WithRecipient(message.Envelope!.SourceNumber!)
                            .WithMessage($"You said: {message.Envelope?.DataMessage.Message}")
                        , cancellationToken: ct);
                }
            },
            async (botClient, error, ct) =>
            {
                Console.WriteLine($"Error: {error.Source}:{error.Exception?.Message}");
                await Task.CompletedTask;
            }, builder => builder.WithMaxMessages(1), cts.Token);

        // Keep the application running
        Console.WriteLine("Bot is running. Press any key to stop...");
        Console.ReadKey();
        cts.Cancel();
    }

    public void QuickExample()
    {
        #region QuickExample
        const string baseUrl = "http://localhost:8080";
        // Your registered Signal number
        const string botNumber = "+1234567890";

        var client = new SignalBotClient(builder =>
        {
            builder
                .WithBaseUrl(baseUrl)
                .WithNumber(botNumber);
        });

        using var cts = new CancellationTokenSource();

        client.StartReceiving(async (botClient, message, ct) =>
            {
                Console.WriteLine($"Received from {message.Account}: {message.Envelope?.DataMessage?.Message}");

                // Echo the message back
                if (!string.IsNullOrEmpty(message.Envelope?.DataMessage?.Message))
                {
                    await botClient.SendMessageAsync(
                        builder => builder
                            .WithRecipient(message.Envelope!.SourceNumber!)
                            .WithMessage($"You said: {message.Envelope?.DataMessage.Message}")
                        , cancellationToken: ct);
                }
            },
            async (botClient, error, ct) =>
            {
                Console.WriteLine($"Error: {error.Source}:{error.Exception?.Message}");
                await Task.CompletedTask;
            }, builder => builder.WithMaxMessages(1), cts.Token);
        #endregion QuickExample
    }
}