namespace Signal.Bot.Example.Guide;

public class SendingMessages
{
    public async Task BasicMessageReceiving()
    {
        #region BasicMessageReceiving
        var botNumber = "+1234567890";
        var client = new SignalBotClient(builder => builder
                    .WithNumber(botNumber)
                    .WithBaseUrl("http://localhost:8080"));

        using var cts = new CancellationTokenSource();

        client.StartReceiving((botClient, message, ct) =>
        {
            if (message.Envelope is not null)
            {
                Console.WriteLine($"Received: {message.Envelope.DataMessage?.Message}");
            }
        }, (botClient, err, ct) =>
        {
            if (err.Exception is not null)
            {
                Console.WriteLine($"Error: {err.Exception.Message}");
            }
        }, cancellationToken: cts.Token);
        #endregion BasicMessageReceiving
    }
}