using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to verify a phone number registration using the verification code received via SMS or voice call.
/// </summary>
/// <param name="Number">The phone number being verified.</param>
/// <param name="Token">The verification code received from Signal.</param>
public record VerifyNumberRequest(string Number, string Token)
    : RequestBase<string>($"v1/register/{Number}/verify/{Token}")
{
    /// <summary>
    /// Gets or sets the registration lock PIN if the Signal account has PIN protection enabled.
    /// </summary>
    [JsonPropertyName("pin")] 
    public string? Pin { get; set; }
}