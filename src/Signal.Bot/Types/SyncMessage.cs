using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a synchronization message sent between linked devices, containing read receipt information.
/// </summary>
public class SyncMessage
{
    /// <summary>
    /// Gets or sets the collection of read receipts to be synchronized across devices.
    /// </summary>
    [JsonPropertyName("readMessages")] 
    public ICollection<ReadMessage>? ReadMessages { get; set; }
}