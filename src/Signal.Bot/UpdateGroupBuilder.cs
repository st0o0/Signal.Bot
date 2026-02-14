using Signal.Bot.Requests;

namespace Signal.Bot;

/// <summary>
/// Provides a fluent interface for building and configuring <see cref="UpdateGroupRequest"/> objects to update Signal group settings.
/// </summary>
public class UpdateGroupBuilder
{
    private readonly UpdateGroupRequest _request;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateGroupBuilder"/> class with default admin-only permissions.
    /// </summary>
    /// <param name="number">The phone number of the Signal account updating the group.</param>
    /// <param name="groupId">The unique identifier of the group to update.</param>
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

    /// <summary>
    /// Sets the name of the group.
    /// </summary>
    /// <param name="name">The new name for the group.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description text for the group.
    /// </summary>
    /// <param name="description">The new description for the group.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithDescription(string description)
    {
        _request.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the disappearing message timer for the group.
    /// </summary>
    /// <param name="expirationTime">The expiration time in seconds. Set to 0 to disable disappearing messages.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithExpirationTime(int expirationTime)
    {
        _request.ExpirationTime = expirationTime;
        return this;
    }

    /// <summary>
    /// Sets the group link access level.
    /// </summary>
    /// <param name="link">The <see cref="GroupLink"/> setting controlling how users can join via link.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithGroupLink(GroupLink link)
    {
        _request.GroupLink = link;
        return this;
    }

    /// <summary>
    /// Sets the permission level for adding new members to the group.
    /// </summary>
    /// <param name="addMember">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithAddMemberPermission(GroupPermission addMember)
    {
        _request.Permissions!.AddMembers = addMember;
        return this;
    }

    /// <summary>
    /// Sets the permission level for editing group information (name, description, avatar).
    /// </summary>
    /// <param name="editGroup">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithEditGroupPermission(GroupPermission editGroup)
    {
        _request.Permissions!.EditGroup = editGroup;
        return this;
    }

    /// <summary>
    /// Sets the permission level for sending messages in the group.
    /// </summary>
    /// <param name="sendMessages">The <see cref="GroupPermission"/> level (e.g., OnlyAdmins or EveryMember).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithSendMessagesPermission(GroupPermission sendMessages)
    {
        _request.Permissions!.SendMessages = sendMessages;
        return this;
    }

    /// <summary>
    /// Sets the group avatar image from a byte array.
    /// </summary>
    /// <param name="avatar">The avatar image as a byte array (JPEG or PNG format recommended).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UpdateGroupBuilder WithAvatar(byte[] avatar)
    {
        _request.Avatar = Base64String.FromBytes(avatar);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="UpdateGroupRequest"/>.
    /// </summary>
    /// <returns>The configured <see cref="UpdateGroupRequest"/> instance.</returns>
    internal UpdateGroupRequest Build() => _request;
}