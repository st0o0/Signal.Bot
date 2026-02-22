using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a linked device associated with a Signal account.
/// </summary>
public record Device
{
    /// <summary>
    /// Gets or sets the name assigned to the device.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the device was linked to the account.
    /// </summary>
    [JsonPropertyName("creation_timestamp")] 
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the device was last active.
    /// </summary>
    [JsonPropertyName("last_seen_timestamp")] 
    public DateTime LastSeen { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}