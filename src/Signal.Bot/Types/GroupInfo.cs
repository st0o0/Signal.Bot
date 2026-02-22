using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents information about a Signal v2 group in a message context.
/// </summary>
public record GroupInfo
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    [JsonPropertyName("groupId")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [JsonPropertyName("groupName")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the revision number of the group state, incremented with each group update.
    /// </summary>
    [JsonPropertyName("revision")]
    public int? Revision { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}