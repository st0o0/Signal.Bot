namespace Signal.Bot.Example.Guide;

public class Groups
{
    private readonly SignalBotClient client = null!;

    public async Task CreatingGroup()
    {
        #region CreatingGroup
        await client.CreateGroupAsync(builder => builder
                .WithName("My Bot Group")
                .WithMembers(["+1111111111", "+2222222222", "+3333333333"]));
        #endregion CreatingGroup
    }

    public async Task WithDescription()
    {
        #region WithDescription
        await client.CreateGroupAsync(builder => builder
                .WithName("Project Team")
                .WithMembers(["+1111111111", "+2222222222"])
                .WithDescription("Our awesome project team"));
        #endregion WithDescription
    }

    public async Task ListingGroups()
    {
        #region ListingGroups
        var groups = await client.GetGroupsAsync();

        foreach (var group in groups)
        {
            Console.WriteLine($"Group: {group.Name}");
            Console.WriteLine($"  ID: {group.Id}");
            Console.WriteLine($"  Members: {group.Members?.Count ?? 0}");
            Console.WriteLine();
        }
        #endregion ListingGroups
    }

    public async Task GettingGroupDetails()
    {
        #region GettingGroupDetails
        var groupId = "your-group-id-here";
        var groupInfo = await client.GetGroupAsync(groupId);

        Console.WriteLine($"Name: {groupInfo.Name}");
        Console.WriteLine($"Description: {groupInfo.Description}");
        Console.WriteLine($"Members: {groupInfo.Members?.Count ?? 0}");
        #endregion GettingGroupDetails
    }

    public async Task UpdateGroup()
    {
        #region UpdateGroup
        var groupId = "your-group-id-here";
        var avatarBytes = await File.ReadAllBytesAsync("/path/to/new-avatar.jpg");
        await client.UpdateGroupAsync(groupId,
                builder => builder.WithName("Servus")
                            .WithDescription("New group description")
                            .WithAvatar(avatarBytes));
        #endregion UpdateGroup
    }

    public async Task GroupAddMember()
    {
        #region AddMember
        var groupId = "your-group-id-here";
        var member = "+0987654321";
        await client.AddGroupMemberAsync(groupId, [member]);
        #endregion AddMember
    }

    public async Task GroupRemoveMember()
    {
        #region RemoveMember
        var groupId = "your-group-id-here";
        var member = "+0987654321";
        await client.RemoveGroupMemberAsync(groupId, [member]);
        #endregion RemoveMember
    }

    public async Task GroupAddAdmin()
    {
        #region AddAdmin
        var groupId = "your-group-id-here";
        var member = "+0987654321";
        await client.AddGroupAdminAsync(groupId, [member]);
        #endregion AddAdmin
    }

    public async Task GroupRemoveAdmin()
    {
        #region RemoveAdmin
        var groupId = "your-group-id-here";
        var member = "+0987654321";
        await client.RemoveGroupAdminAsync(groupId, [member]);
        #endregion RemoveAdmin
    }

    public async Task QuitGroup()
    {
        #region QuitGroup
        var groupId = "your-group-id-here";
        await client.QuitGroupAsync(groupId);
        #endregion QuitGroup
    }

    public async Task DeletingGroup()
    {
        #region DeletingGroup
        var groupId = "your-group-id-here";
        await client.RemoveGroupAsync(groupId);
        #endregion DeletingGroup
    }

    public async Task SendGroupMessage()
    {
        #region SendGroupMessage
        var groupId = "your-group-id-here";
        await client.SendMessageAsync(builder => builder
                    .WithMessage("Hello everyone!")
                    .WithRecipient(groupId));
        #endregion SendGroupMessage
    }
}