using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the registration status result for a searched phone number.
/// </summary>
public class Search
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
}