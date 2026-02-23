using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a mention of a user within a message, specifying their position in the text.
/// </summary>
public record Mention
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
    
    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }
    
    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}