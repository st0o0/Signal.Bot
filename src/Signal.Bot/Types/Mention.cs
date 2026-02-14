using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a mention of a user within a message, specifying their position in the text.
/// </summary>
public class Mention
{
    /// <summary>
    /// Gets or sets the zero-based starting character position of the mention in the message text.
    /// </summary>
    [JsonPropertyName("start")] 
    public int? Start { get; set; }

    /// <summary>
    /// Gets or sets the length of the mention text in characters.
    /// </summary>
    [JsonPropertyName("length")] 
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier (UUID) of the user being mentioned.
    /// </summary>
    [JsonPropertyName("uuid")] 
    public Guid Id { get; set; }
}