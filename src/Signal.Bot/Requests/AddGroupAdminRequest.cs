namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to add administrators to a Signal group.
/// </summary>
/// <param name="Number">The phone number of the account making the request.</param>
/// <param name="GroupId">The ID of the group to which administrators will be added.</param>
public record AddGroupAdminRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}/admins")
{
    /// <summary>
    /// Gets or sets the collection of phone numbers to be added as group administrators.
    /// </summary>
    [JsonPropertyName("admins")] public string[]? Admins { get; set; }
}