using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the nickname components for a contact, including given name and family name.
/// </summary>
public class Nickname
{
    /// <summary>
    /// Gets or sets the family name (last name) component of the nickname.
    /// </summary>
    [JsonPropertyName("family_name")] 
    public string? FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the given name (first name) component of the nickname.
    /// </summary>
    [JsonPropertyName("given_name")] 
    public string? GivenName { get; set; }

    /// <summary>
    /// Gets or sets the full nickname.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }
}