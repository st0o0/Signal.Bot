using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a vote on a poll message.
/// </summary>
public record PollVote
{
    /// <summary>
    /// Gets or sets the author identifier.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the vote author.
    /// </summary>
    [JsonPropertyName("authorNumber")]
    public string? AuthorNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the vote author.
    /// </summary>
    [JsonPropertyName("authorUuid")]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the indexes of the selected poll options.
    /// </summary>
    [JsonPropertyName("optionIndexes")]
    public List<int>? OptionIndexes { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the original poll message.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }

    /// <summary>
    /// Gets or sets the total number of votes on this poll.
    /// </summary>
    [JsonPropertyName("voteCount")]
    public int VoteCount { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
