using Signal.Bot.Requests;

namespace Signal.Bot;

/// <summary>
/// Provides a fluent interface for building and configuring <see cref="CreateGroupRequest"/> objects to create new Signal groups.
/// </summary>
public class CreateGroupBuilder
{
    private readonly CreateGroupRequest _request;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateGroupBuilder"/> class with default admin-only permissions.
    /// </summary>
    /// <param name="number">The phone number of the Signal account creating the group.</param>
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

    /// <summary>
    /// Sets the name of the group.
    /// </summary>
    /// <param name="name">The name for the new group.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description text for the group.
    /// </summary>
    /// <param name="description">The description for the new group.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithDescription(string description)
    {
        _request.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the disappearing message timer for the group.
    /// </summary>
    /// <param name="expirationTime">The expiration time in seconds. Set to 0 to disable disappearing messages.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithExpirationTime(int expirationTime)
    {
        _request.ExpirationTime = expirationTime;
        return this;
    }

    /// <summary>
    /// Sets the group link access level.
    /// </summary>
    /// <param name="link">The <see cref="GroupLink"/> setting controlling how users can join via link.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithGroupLink(GroupLink link)
    {
        _request.GroupLink = link;
        return this;
    }

    /// <summary>
    /// Sets the permission level for adding new members to the group.
    /// </summary>
    /// <param name="addMember">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithAddMemberPermission(GroupPermission addMember)
    {
        _request.Permissions!.AddMembers = addMember;
        return this;
    }

    /// <summary>
    /// Sets the permission level for editing group information (name, description, avatar).
    /// </summary>
    /// <param name="editGroup">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithEditGroupPermission(GroupPermission editGroup)
    {
        _request.Permissions!.EditGroup = editGroup;
        return this;
    }

    /// <summary>
    /// Sets the permission level for sending messages in the group.
    /// </summary>
    /// <param name="sendMessages">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithSendMessagesPermission(GroupPermission sendMessages)
    {
        _request.Permissions!.SendMessages = sendMessages;
        return this;
    }

    /// <summary>
    /// Sets the initial members of the group.
    /// </summary>
    /// <param name="members">The phone numbers of users to add as initial group members.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CreateGroupBuilder WithMembers(IEnumerable<string> members)
    {
        _request.Members = members.ToArray();
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="CreateGroupRequest"/>.
    /// </summary>
    /// <returns>The configured <see cref="CreateGroupRequest"/> instance.</returns>
    internal CreateGroupRequest Build() => _request;
}