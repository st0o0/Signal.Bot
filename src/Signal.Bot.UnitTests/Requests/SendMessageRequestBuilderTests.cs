using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Requests;

public class SendMessageRequestBuilderTests
{
    [Fact(Timeout = 5000)]
    public void Build_EmptyBuilder_ReturnsDefaultRequest()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.Build();

        Assert.NotNull(request);
        Assert.Null(request.Message);
        Assert.Null(request.Recipients);
    }

    [Fact(Timeout = 5000)]
    public void WithMessage_SetsMessage()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.WithMessage("Hello").Build();

        Assert.Equal("Hello", request.Message);
    }

    [Fact(Timeout = 5000)]
    public void WithRecipient_AddsRecipient()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.WithRecipient("user1").Build();

        Assert.Contains("user1", request.Recipients!);
    }

    [Fact(Timeout = 5000)]
    public void WithRecipients_SetsRecipients()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var recipients = new[] { "user1", "user2" };
        var request = builder.WithRecipients(recipients).Build();

        Assert.Equal(2, request.Recipients!.Length);
        Assert.Equal("user1", request.Recipients[0]);
        Assert.Equal("user2", request.Recipients[1]);
    }

    [Fact(Timeout = 5000)]
    public void WithRecipients_AppendMode_AppendsRecipients()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder
            .WithRecipient("user1")
            .WithRecipients(["user2", "user3"], overwrite: false)
            .Build();

        Assert.Equal(3, request.Recipients!.Length);
        Assert.Equal("user1", request.Recipients[0]);
        Assert.Equal("user2", request.Recipients[1]);
        Assert.Equal("user3", request.Recipients[2]);
    }

    [Fact(Timeout = 5000)]
    public void WithMention_AddsMention()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.WithMention("author1", 0, 5).Build();

        Assert.Single(request.Mentions!);
        Assert.Equal("author1", request.Mentions![0].Author);
        Assert.Equal(0, request.Mentions[0].Start);
        Assert.Equal(5, request.Mentions[0].Length);
    }

    [Fact(Timeout = 5000)]
    public void WithLinkPreview_SetsLinkPreview()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.WithLinkPreview("https://example.com", "Title", "Desc").Build();

        Assert.NotNull(request.LinkPreview);
        Assert.Equal("https://example.com", request.LinkPreview.Url);
        Assert.Equal("Title", request.LinkPreview.Title);
        Assert.Equal("Desc", request.LinkPreview.Description);
    }

    [Fact(Timeout = 5000)]
    public void WithAttachment_AddsAttachment()
    {
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder.WithAttachment("base64data").WithSticker("KEKW").WithViewOnce()
            .WithTextMode(TextMode.Styled).Build();

        Assert.Contains("base64data", request.Attachments!);
        Assert.True(request.ViewOnce);
        Assert.Equal(TextMode.Styled, request.TextMode);
        Assert.Equal("KEKW", request.Sticker);
    }

    [Fact(Timeout = 5000)]
    public void WithQuote_SetsQuoteInfo()
    {
        var timestamp = DateTime.UtcNow;
        var builder = SendMessageRequestBuilder.Create(string.Empty);
        var request = builder
            .WithQuoteAuthor("author")
            .WithQuoteMessage("text")
            .WithQuoteTimestamp(timestamp)
            .Build();

        Assert.Equal("author", request.QuoteAuthor);
        Assert.Equal("text", request.QuoteMessage);
        Assert.Equal(timestamp, request.QuoteTimestamp);
    }
}