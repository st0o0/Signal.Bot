namespace Signal.Bot.Example;

public class Overview
{
    public async Task SendAMessage()
    {
        #region SendAMessage

        var client = new SignalBotClient(builder => builder
            .WithBaseUrl("http://localhost:8080")
            .WithNumber("+1234567890"));

        await client.SendMessageAsync(builder =>
            builder
                .WithNumber("+1234567890")
                .WithMessage("Hello!")
                .WithRecipient("+0987654321"));

        #endregion
    }
}