using Signal.Bot.Requests;

namespace Signal.Bot.Example.DocuCode;

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
                .WithMessage("Hello!")
                .WithRecipient("+0987654321"));

        #endregion SendAMessage
    }

    public async Task ReceiveMessages(CancellationToken cancellationToken = default)
    {
        var client = new SignalBotClient(builder => builder
            .WithBaseUrl("http://localhost:8080")
            .WithNumber("+1234567890"));

        #region ReceiveMessages

        var disposable = await client.ReceiveAsync(
            updateHandler: (client, message, ct) =>
            {
                Console.WriteLine($"Received: {message.Envelope?.DataMessage?.Message}");
            },
            errorHandler: (client, ex, ct) => { Console.WriteLine($"Error: {ex.Exception?.Message ?? ""}"); },
            cancellationToken: cancellationToken);

        #endregion ReceiveMessages
    }

    public async Task CreateAGroup(CancellationToken cancellationToken = default)
    {
        var client = new SignalBotClient(builder => builder
            .WithBaseUrl("http://localhost:8080")
            .WithNumber("+1234567890"));

        #region CreateAGroup

        var group = await client.CreateGroupAsync(builder =>
        {
            builder
                .WithAddMemberPermission(GroupPermission.OnlyAdmins)
                .WithEditGroupPermission(GroupPermission.OnlyAdmins)
                .WithSendMessagesPermission(GroupPermission.OnlyAdmins)
                .WithMembers(["+1111111111", "+2222222222"])
                .WithName("My Group");
        }, cancellationToken);

        #endregion CreateAGroup
    }

    public async Task SendAttachment(CancellationToken cancellationToken = default)
    {
        var client = new SignalBotClient(builder => builder
            .WithBaseUrl("http://localhost:8080")
            .WithNumber("+1234567890"));

        #region SendAttachment

        await client.SendMessageAsync(builder =>
                builder
                    .WithMessage("Check this out!")
                    .WithRecipient("+0987654321")
                    .WithAttachmentFromFile("/path/to/file.jpg", includeFilename: true),
            cancellationToken);

        #endregion SendAttachment
    }
}