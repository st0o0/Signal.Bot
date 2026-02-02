using Signal.Bot.Requests;

namespace Signal.Bot;

public class CreateGroupBuilder
{
    private readonly CreateGroupRequest _request;

    public CreateGroupBuilder(string number)
    {
        _request = new CreateGroupRequest(number)
        {
            Permissions = new Permissions
            {
                AddMembers = GroupPermission.OnlyAdmins,
                EditGroup = GroupPermission.OnlyAdmins,
                SendMessages = GroupPermission.OnlyAdmins
            }
        };
    }

    public CreateGroupBuilder WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    public CreateGroupBuilder WithDescription(string description)
    {
        _request.Description = description;
        return this;
    }

    public CreateGroupBuilder WithExpirationTime(int expirationTime)
    {
        _request.ExpirationTime = expirationTime;
        return this;
    }

    public CreateGroupBuilder WithGroupLink(GroupLink link)
    {
        _request.GroupLink = link;
        return this;
    }

    public CreateGroupBuilder WithAddMemberPermission(GroupPermission addMember)
    {
        _request.Permissions!.AddMembers = addMember;
        return this;
    }

    public CreateGroupBuilder WithEditGroupPermission(GroupPermission editGroup)
    {
        _request.Permissions!.EditGroup = editGroup;
        return this;
    }

    public CreateGroupBuilder WithSendMessagesPermission(GroupPermission sendMessages)
    {
        _request.Permissions!.SendMessages = sendMessages;
        return this;
    }

    public CreateGroupBuilder WithMembers(IEnumerable<string> members)
    {
        _request.Members = members.ToArray();
        return this;
    }

    internal CreateGroupRequest Build() => _request;
}