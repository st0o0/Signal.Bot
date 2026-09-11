using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents per-recipient error information from a send message response.
/// </summary>
public record SendMessageErrors
{
    /// <summary>
    /// Gets or sets the list of recipient-specific errors.
    /// </summary>
    [JsonPropertyName("recipients")]
    public List<RecipientError>? Recipients { get; set; }
}

/// <summary>
/// Represents an error for a specific recipient.
/// </summary>
public record RecipientError
{
    /// <summary>
    /// Gets or sets the phone number of the recipient.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the recipient.
    /// </summary>
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    /// <summary>
    /// Gets or sets the username of the recipient.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the reason for the error.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
