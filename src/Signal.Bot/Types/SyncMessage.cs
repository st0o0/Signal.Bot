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

    /// <summary>
    /// Gets or sets the list of blocked group IDs.
    /// </summary>
    [JsonPropertyName("blockedGroupIds")]
    public List<string>? BlockedGroupIds { get; set; }

    /// <summary>
    /// Gets or sets the list of blocked phone numbers.
    /// </summary>
    [JsonPropertyName("blockedNumbers")]
    public List<string>? BlockedNumbers { get; set; }

    /// <summary>
    /// Gets or sets the synced story message sent from a linked device.
    /// </summary>
    [JsonPropertyName("sentStoryMessage")]
    public SyncStoryMessage? SentStoryMessage { get; set; }

    /// <summary>
    /// Gets or sets the sync message type.
    /// </summary>
    [JsonPropertyName("type")]
    public SyncMessageType? Type { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Defines the types of synchronization messages between linked devices.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<SyncMessageType>))]
public enum SyncMessageType
{
    /// <summary>Contacts synchronization.</summary>
    [JsonStringEnumMemberName("CONTACTS_SYNC")] ContactsSync,

    /// <summary>Groups synchronization.</summary>
    [JsonStringEnumMemberName("GROUPS_SYNC")] GroupsSync,

    /// <summary>Sync request from another device.</summary>
    [JsonStringEnumMemberName("REQUEST_SYNC")] RequestSync,
}