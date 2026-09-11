using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to get the trust mode settings for a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
public record GetTrustModeRequest(string Number)
    : RequestBase<TrustModeSettings>($"v1/configuration/{Number}/settings", HttpMethod.Get);
