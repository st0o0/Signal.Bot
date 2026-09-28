using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a shared contact in a message.
/// </summary>
public record SharedContact
{
    /// <summary>
    /// Gets or sets the list of addresses for this contact.
    /// </summary>
    [JsonPropertyName("address")]
    public List<ContactAddress>? Address { get; set; }

    /// <summary>
    /// Gets or sets the contact avatar.
    /// </summary>
    [JsonPropertyName("avatar")]
    public ContactAvatar? Avatar { get; set; }

    /// <summary>
    /// Gets or sets the list of email addresses for this contact.
    /// </summary>
    [JsonPropertyName("email")]
    public List<ContactEmail>? Email { get; set; }

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    [JsonPropertyName("name")]
    public ContactName? Name { get; set; }

    /// <summary>
    /// Gets or sets the organization name.
    /// </summary>
    [JsonPropertyName("organization")]
    public string? Organization { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers for this contact.
    /// </summary>
    [JsonPropertyName("phone")]
    public List<ContactPhone>? Phone { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Represents a contact's postal address.
/// </summary>
public record ContactAddress
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country name or code.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the custom label for this address.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the neighborhood name.
    /// </summary>
    [JsonPropertyName("neighborhood")]
    public string? Neighborhood { get; set; }

    /// <summary>
    /// Gets or sets the P.O. box number.
    /// </summary>
    [JsonPropertyName("pobox")]
    public string? Pobox { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    /// <summary>
    /// Gets or sets the region or state.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>
    /// Gets or sets the address type (e.g., HOME, WORK).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Represents a contact's avatar.
/// </summary>
public record ContactAvatar
{
    /// <summary>
    /// Gets or sets the avatar image attachment.
    /// </summary>
    [JsonPropertyName("attachment")]
    public Attachment? Attachment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a profile avatar.
    /// </summary>
    [JsonPropertyName("isProfile")]
    public bool? IsProfile { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Represents a contact's email address.
/// </summary>
public record ContactEmail
{
    /// <summary>
    /// Gets or sets the custom label for this email address.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the email type (e.g., HOME, WORK).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Represents a contact's name.
/// </summary>
public record ContactName
{
    /// <summary>
    /// Gets or sets the family (last) name.
    /// </summary>
    [JsonPropertyName("family")]
    public string? Family { get; set; }

    /// <summary>
    /// Gets or sets the given (first) name.
    /// </summary>
    [JsonPropertyName("given")]
    public string? Given { get; set; }

    /// <summary>
    /// Gets or sets the middle name.
    /// </summary>
    [JsonPropertyName("middle")]
    public string? Middle { get; set; }

    /// <summary>
    /// Gets or sets the nickname.
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// Gets or sets the name prefix (e.g., Dr, Mr).
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets the name suffix (e.g., Jr, III).
    /// </summary>
    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Represents a contact's phone number.
/// </summary>
public record ContactPhone
{
    /// <summary>
    /// Gets or sets the custom label for this phone number.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the phone number type (e.g., MOBILE, HOME, WORK).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
