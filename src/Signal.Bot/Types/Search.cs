using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the registration status result for a searched phone number.
/// </summary>
public record Search
{
    /// <summary>
    /// Gets or sets the phone number that was searched.
    /// </summary>
    [JsonPropertyName("number")] 
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the phone number is registered on Signal.
    /// </summary>
    [JsonPropertyName("registered")] 
    public bool? Registered { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}