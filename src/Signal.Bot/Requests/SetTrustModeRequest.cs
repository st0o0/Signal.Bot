using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to set the trust mode for a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
public record SetTrustModeRequest(string Number)
    : RequestBase<TrustModeSettings>($"v1/configuration/{Number}/settings")
{
    /// <summary>
    /// Gets or sets the trust mode to set.
    /// </summary>
    [JsonPropertyName("trust_mode")]
    public string? TrustMode { get; set; }
}
