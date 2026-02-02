using System.Runtime.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record CreateGroupRequest(string Number) : RequestBase<Group>($"v1/groups/{Number}")
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("expiration_time")] public int? ExpirationTime { get; set; }

    [JsonPropertyName("group_link")] public GroupLink GroupLink { get; set; }

    [JsonPropertyName("permissions")] public Permissions Permissions { get; set; }

    [JsonPropertyName("members")] public string[]? Members { get; set; }
}

public class Permissions
{
    [JsonPropertyName("add_members")] public GroupPermission AddMembers { get; set; }

    [JsonPropertyName("edit_group")] public GroupPermission EditGroup { get; set; }

    [JsonPropertyName("send_messages")] public GroupPermission SendMessages { get; set; }
}

public enum GroupPermission
{
    [EnumMember(Value = "")] Undefined = 0,

    [EnumMember(Value = "only-admins")] OnlyAdmins = 0,

    [EnumMember(Value = "every-member")] EveryMember = 1,
}

public enum GroupLink
{
    [EnumMember(Value = "")] Undefined = 0,

    [EnumMember(Value = "disabled")] Disabled = 1,

    [EnumMember(Value = "enabled")] Enabled = 2,

    [EnumMember(Value = "enabled-with-approval")] EnabledWithApproval = 3,
}