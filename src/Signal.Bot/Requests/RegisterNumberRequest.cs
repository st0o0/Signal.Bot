using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to initiate the registration process for a phone number with Signal.
/// </summary>
/// <param name="Number">The phone number to register in international format (e.g., "+1234567890").</param>
public record RegisterNumberRequest(string Number) : RequestBase($"v1/register/{Number}")
{
    /// <summary>
    /// Gets or sets the CAPTCHA token if required by Signal to prevent automated registrations.
    /// </summary>
    [JsonPropertyName("captcha")] 
    public string? Captcha { get; set; }
    
    /// <summary>
    /// Gets or sets whether to use a voice call instead of SMS for receiving the verification code. Default is <see langword="false"/>.
    /// </summary>
    [JsonPropertyName("use_voice")] 
    public bool? UseVoice { get; set; }
}