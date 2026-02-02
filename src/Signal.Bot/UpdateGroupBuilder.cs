using Signal.Bot.Requests;

namespace Signal.Bot;

public class UpdateGroupBuilder
{
    private readonly UpdateGroupRequest _request;

    public UpdateGroupBuilder(string number, string groupId)
    {
        _request = new UpdateGroupRequest(number, groupId)
        {
            Permissions = new Permissions
            {
                AddMembers = GroupPermission.OnlyAdmins,
                EditGroup = GroupPermission.OnlyAdmins,
                SendMessages = GroupPermission.OnlyAdmins
            }
        };
    }

    public UpdateGroupBuilder WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    public UpdateGroupBuilder WithDescription(string description)
    {
        _request.Description = description;
        return this;
    }

    public UpdateGroupBuilder WithExpirationTime(int expirationTime)
    {
        _request.ExpirationTime = expirationTime;
        return this;
    }

    public UpdateGroupBuilder WithGroupLink(GroupLink link)
    {
        _request.GroupLink = link;
        return this;
    }

    public UpdateGroupBuilder WithAddMemberPermission(GroupPermission addMember)
    {
        _request.Permissions!.AddMembers = addMember;
        return this;
    }

    public UpdateGroupBuilder WithEditGroupPermission(GroupPermission editGroup)
    {
        _request.Permissions!.EditGroup = editGroup;
        return this;
    }

    public UpdateGroupBuilder WithSendMessagesPermission(GroupPermission sendMessages)
    {
        _request.Permissions!.SendMessages = sendMessages;
        return this;
    }

    public UpdateGroupBuilder WithAvatar(byte[] avatar)
    {
        _request.Avatar = Base64String.FromBytes(avatar);
        return this;
    }

    internal UpdateGroupRequest Build() => _request;
}