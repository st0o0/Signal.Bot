using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a synchronization message sent between linked devices, containing read receipt information.
/// </summary>
public record SyncMessage
{
    /// <summary>A copy of a message the local user sent (used for multi-device sync).</summary>
    [JsonPropertyName("sentMessage")]
    public SentMessage? SentMessage { get; set; }

    /// <summary>
    /// Gets or sets the collection of read receipts to be synchronized across devices.
    /// </summary>
    [JsonPropertyName("readMessages")]
    public List<ReadMessage>? ReadMessages { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}