using Moq;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

public class SignalBotClientExtensionsTests
{
    private readonly Mock<ISignalBotClient> _client = new(MockBehavior.Strict);

    [Fact]
    public async Task SendMessageAsync_SendsCorrectRequest()
    {
        // Arrange
        var expectedResponse = new SendMessage();
        _client
            .Setup(c => c.SendRequestAsync(
                It.Is<SendMessageRequest>(r =>
                    r.Message == "hello" &&
                    r.Number == "123" &&
                    r.Recipients.Count == 1 &&
                    r.Recipients.Contains("456")),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _client.Object.SendMessageAsync(
            recipient: "456",
            message: "hello");

        // Assert
        Assert.Same(expectedResponse, result);
        _client.VerifyAll();
    }

    [Fact]
    public async Task GetAboutAsync_SendsGetAboutRequest()
    {
        // Arrange
        var about = new About();
        _client
            .Setup(c => c.SendRequestAsync(It.IsAny<GetAboutRequest>(), It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(about);

        // Act
        var result = await _client.Object.GetAboutAsync();

        // Assert
        Assert.Same(about, result);
        _client.Verify(c =>
                c.SendRequestAsync(It.IsAny<GetAboutRequest>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterNumberAsync_SetsOptionalValuesCorrectly()
    {
        // Arrange
        _client
            .Setup(c => c.SendRequestAsync(
                It.Is<RegisterNumberRequest>(r =>
                    r.Number == "123" &&
                    r.Captcha == "captcha-token" &&
                    r.UseVoice == true), It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.Object.RegisterNumberAsync(
            captcha: "captcha-token",
            useVoice: true);

        // Assert
        _client.VerifyAll();
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_WhenIsTypingFalse_UsesRemoveRequest()
    {
        // Arrange
        _client
            .Setup(c => c.SendRequestAsync(
                It.Is<RemoveTypingIndicatorRequest>(r =>
                    r.Number == "123" &&
                    r.Recipient == "456"), It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.Object.SetTypingIndicatorAsync(
            recipient: "456",
            isTyping: false);

        // Assert
        _client.VerifyAll();
    }

    [Fact]
    public async Task AddGroupMemberAsync_SendsCorrectMembers()
    {
        // Arrange
        var members = new List<string> { "a", "b" };

        _client
            .Setup(c => c.SendRequestAsync(
                It.Is<AddGroupMemberRequest>(r =>
                    r.Number == "123" &&
                    r.GroupId == "group1" &&
                    r.Members == members), It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.Object.AddGroupMemberAsync(
            groupId: "group1",
            members: members);

        // Assert
        _client.VerifyAll();
    }
}