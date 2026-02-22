using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a Signal contact with profile information and settings.
/// </summary>
public record Contact
{
    /// <summary>
    /// Gets or sets the unique identifier (UUID) of the contact.
    /// </summary>
    [JsonPropertyName("uuid")] 
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the contact in international format.
    /// </summary>
    [JsonPropertyName("number")] 
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the locally assigned name for this contact.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the profile name set by the contact themselves.
    /// </summary>
    [JsonPropertyName("profile_name")] 
    public string? ProfileName { get; set; }

    /// <summary>
    /// Gets or sets the Signal username of the contact, if they have set one.
    /// </summary>
    [JsonPropertyName("username")] 
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the color associated with this contact in the UI.
    /// </summary>
    [JsonPropertyName("color")] 
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this contact is blocked.
    /// </summary>
    [JsonPropertyName("blocked")] 
    public bool Blocked { get; set; }

    /// <summary>
    /// Gets or sets the disappearing message timer setting for conversations with this contact.
    /// </summary>
    [JsonPropertyName("message_expiration")] 
    public string? MessageExpiration { get; set; }

    /// <summary>
    /// Gets or sets a personal note about this contact.
    /// </summary>
    [JsonPropertyName("note")] 
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the Signal profile information for this contact.
    /// </summary>
    [JsonPropertyName("profile")] 
    public Profile? Profile { get; set; }

    /// <summary>
    /// Gets or sets the given (first) name of the contact.
    /// </summary>
    [JsonPropertyName("given_name")] 
    public string? GivenName { get; set; }

    /// <summary>
    /// Gets or sets the nickname information for this contact.
    /// </summary>
    [JsonPropertyName("nickname")] 
    public Nickname? Nickname { get; set; }
}