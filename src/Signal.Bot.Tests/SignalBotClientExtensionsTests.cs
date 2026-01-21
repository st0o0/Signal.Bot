using NSubstitute;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

public class SignalBotClientExtensionsTests
{
    private readonly ISignalBotClient _client;

    public SignalBotClientExtensionsTests()
    {
        _client = Substitute.For<ISignalBotClient>();
        _client.Number.Returns("123");
    }

    [Fact]
    public async Task SendMessageAsync_SendsCorrectRequest()
    {
        // Arrange
        var expectedResponse = new SendMessage();
        _client
            .SendRequestAsync(Arg.Is<SendMessageRequest>(r =>
                    r.Message == "hello" &&
                    r.Number == "123" &&
                    r.Recipients!.Count == 1 &&
                    r.Recipients.Contains("456")),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _client.SendMessageAsync(
            recipient: "456",
            message: "hello");

        // Assert
        Assert.Same(expectedResponse, result);
        await _client.Received(1).SendRequestAsync(Arg.Any<SendMessageRequest>(), Arg.Any<string[]>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAboutAsync_SendsGetAboutRequest()
    {
        // Arrange
        var about = new About();
        _client
            .SendRequestAsync(Arg.Any<GetAboutRequest>(),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(about);

        // Act
        var result = await _client.GetAboutAsync();

        // Assert
        Assert.Same(about, result);
        await _client.Received(1)
            .SendRequestAsync(Arg.Any<GetAboutRequest>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterNumberAsync_SetsOptionalValuesCorrectly()
    {
        // Arrange
        _client
            .SendRequestAsync(Arg.Is<RegisterNumberRequest>(r =>
                    r.Number == "123" &&
                    r.Captcha == "captcha-token" &&
                    r.UseVoice == true),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _client.RegisterNumberAsync(
            captcha: "captcha-token",
            useVoice: true);

        // Assert
        await _client.Received(1).SendRequestAsync(Arg.Any<RegisterNumberRequest>(), Arg.Any<string[]>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_WhenIsTypingFalse_UsesRemoveRequest()
    {
        // Arrange
        _client
            .SendRequestAsync(Arg.Is<RemoveTypingIndicatorRequest>(r =>
                    r.Number == "123" &&
                    r.Recipient == "456"),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _client.SetTypingIndicatorAsync(
            recipient: "456",
            isTyping: false);

        // Assert
        await _client.Received(1).SendRequestAsync(Arg.Any<RemoveTypingIndicatorRequest>(), Arg.Any<string[]>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddGroupMemberAsync_SendsCorrectMembers()
    {
        // Arrange
        var members = new List<string> { "a", "b" };

        _client
            .SendRequestAsync(Arg.Is<AddGroupMemberRequest>(r =>
                    r.Number == "123" &&
                    r.GroupId == "group1" &&
                    r.Members == members),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _client.AddGroupMemberAsync(
            groupId: "group1",
            members: members);

        // Assert
        await _client.Received(1).SendRequestAsync(Arg.Any<AddGroupMemberRequest>(), Arg.Any<string[]>(),
            Arg.Any<CancellationToken>());
    }
}