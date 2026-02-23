using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// TBD
/// </summary>
public record SentMessage
{
    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("destination")]
    public string? Destination { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("destinationNumber")]
    public string? DestinationNumber { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("destinationUuid")]
    public Guid? DestinationId { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("expiresInSeconds")]
    public TimeSpan? ExpiresIn { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("isExpirationUpdate")]
    public bool? IsExpirationUpdate { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("viewOnce")]
    public bool? ViewOnce { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("reaction")]
    public Reaction? Reaction { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("groupInfo")]
    public GroupInfo? GroupInfo { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("previews")]
    public List<Preview>? Previews { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("remoteDelete")]
    public Acknowledged? RemoteDelete { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}