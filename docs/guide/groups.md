# Working with Groups

Signal.Bot provides comprehensive support for managing Signal groups.

## Creating Groups

Create a new Signal group with members:

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#CreatingGroup{csharp}

### With Description

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#WithDescription{csharp}

## Listing Groups

Get all groups the bot is a member of:

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#ListingGroups{csharp}

## Getting Group Details

Retrieve detailed information about a specific group:

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#GettingGroupDetails{csharp}

## Updating Groups

### Update Group

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#UpdateGroup{csharp}

### Add Members

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#AddMember{csharp}

### Remove Members

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#RemoveMember{csharp}

### Make Admins

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#RemoveAdmin{csharp}

## Leaving Groups

Leave a group:

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#QuitGroup{csharp}

## Deleting Groups

Delete a group (admin only):

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#DeletingGroup{csharp}


## Sending Messages to Groups

Send a message to a group:

<<< ./../../src/Signal.Bot.Example/Guide/Groups.cs#SendGroupMessage{csharp}

## Next Steps

- Learn about [attachments](/guide/attachments)
- Explore [profile management](/guide/profiles)
- Check out [advanced examples](/examples/)