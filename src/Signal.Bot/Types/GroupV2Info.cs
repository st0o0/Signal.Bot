using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents information about a Signal v2 group in a message context.
/// </summary>
public class GroupV2Info
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    [JsonPropertyName("id")] 
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the revision number of the group state, incremented with each group update.
    /// </summary>
    [JsonPropertyName("revision")] 
    public int? Revision { get; set; }
}