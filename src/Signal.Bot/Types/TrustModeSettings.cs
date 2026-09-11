using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the trust mode settings for a Signal account.
/// </summary>
public record TrustModeSettings
{
    /// <summary>
    /// Gets or sets the trust mode for this account.
    /// </summary>
    [JsonPropertyName("trust_mode")]
    public string? TrustMode { get; set; }
}
