using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot;

public static partial class SignalBotClientExtensions
{
    public static async Task<SendMessage> SendMessageAsync(
        this ISignalBotClient client,
        string recipient,
        string message,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new SendMessageRequest
        {
            Recipients = new List<string> { recipient },
            Message = message,
            Number = client.Number
        }, cancellationToken: cancellationToken);
    }

    public static async Task<About> GetAboutAsync(
        this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAboutRequest(), cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<string>> GetAccountsAsync(
        this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAccountsRequest(), cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<Group>> GetGroupsAsync(
        this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetGroupsRequest(client.Number), cancellationToken: cancellationToken);
    }

    public static async Task RegisterNumberAsync(
        this ISignalBotClient client,
        string? captcha = null,
        bool? useVoice = null,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RegisterNumberRequest(client.Number)
        {
            Captcha = captcha,
            UseVoice = useVoice
        }, cancellationToken: cancellationToken);
    }

    public static async Task<string> VerifyNumberAsync(
        this ISignalBotClient client,
        string token,
        string? pin = null,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new VerifyNumberRequest(client.Number, token)
        {
            Pin = pin
        }, cancellationToken: cancellationToken);
    }

    public static async Task UpdateProfileAsync(
        this ISignalBotClient client,
        string? name = null,
        string? about = null,
        string? base64Avatar = null,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new UpdateProfileRequest(client.Number)
        {
            Name = name,
            About = about,
            Base64Avatar = base64Avatar
        }, cancellationToken: cancellationToken);
    }

    public static async Task SetTypingIndicatorAsync(
        this ISignalBotClient client,
        string? recipient = null,
        string? groupId = null,
        bool isTyping = true,
        CancellationToken cancellationToken = default)
    {
        var typing = new SetTypingIndicatorRequest(client.Number)
        {
            GroupId = groupId,
            Recipient = recipient
        };
        var resetTyping = new RemoveTypingIndicatorRequest(client.Number)
        {
            GroupId = groupId,
            Recipient = recipient
        };
        await client.SendRequestAsync(isTyping ? typing : resetTyping, cancellationToken: cancellationToken);
    }

    // Account extensions
    public static async Task SetPinAsync(
        this ISignalBotClient client,
        string pin,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new SetPinRequest(client.Number) { Pin = pin },
            cancellationToken: cancellationToken);
    }

    public static async Task RemovePinAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemovePinRequest(client.Number), cancellationToken: cancellationToken);
    }

    public static async Task RateLimitChallengeAsync(
        this ISignalBotClient client,
        string challengeToken,
        string captcha,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RateLimitChallengeRequest(client.Number)
        {
            ChallengeToken = challengeToken,
            Captcha = captcha
        }, cancellationToken: cancellationToken);
    }

    public static async Task UpdateAccountSettingsAsync(
        this ISignalBotClient client,
        bool discoverableByNumber = true,
        bool shareNumberWithContacts = true,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new UpdateAccountSettingsRequest(client.Number)
        {
            DiscoverableByNumber = discoverableByNumber,
            ShareNumberWithContacts = shareNumberWithContacts
        }, cancellationToken: cancellationToken);
    }

    public static async Task<SetUsername> SetUsernameAsync(
        this ISignalBotClient client, string username, CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new SetUsernameRequest(client.Number) { Username = username },
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveUsernameAsync(
        this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemoveUsernameRequest(client.Number), cancellationToken: cancellationToken);
    }

    // Device extensions
    public static async Task<ICollection<Device>> GetDevicesAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetDevicesRequest(client.Number),
            cancellationToken: cancellationToken);
    }

    public static async Task AddDeviceAsync(this ISignalBotClient client, string uri,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new AddDeviceRequest(client.Number) { Uri = uri },
            cancellationToken: cancellationToken);
    }

    public static async Task UnregisterDeviceAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new UnregisterDeviceRequest(client.Number), cancellationToken: cancellationToken);
    }

    // Content extensions
    public static async Task<ICollection<string>> GetAttachmentsAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAttachmentsRequest(), cancellationToken: cancellationToken);
    }

    public static async Task<string> GetAttachmentAsync(
        this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAttachmentRequest(attachmentId),
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveAttachmentAsync(
        this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemoveAttachmentRequest(attachmentId), cancellationToken: cancellationToken);
    }

    public static async Task AddReactionAsync(
        this ISignalBotClient client,
        string reaction,
        string recipient,
        string targetAuthor,
        int timestamp,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new AddReactionRequest(client.Number)
        {
            Reaction = reaction,
            Recipient = recipient,
            TargetAuthor = targetAuthor,
            Timestamp = timestamp
        }, cancellationToken: cancellationToken);
    }

    public static async Task<string> RemoveReactionAsync(
        this ISignalBotClient client,
        string emoji,
        string recipient,
        string targetAuthor,
        int timestamp,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new RemoveReactionRequest(client.Number)
        {
            Emoji = emoji,
            Recipient = recipient,
            TargetAuthor = targetAuthor,
            Timestamp = timestamp
        }, cancellationToken: cancellationToken);
    }

    public static async Task<RemoteDelete> RemoteDeleteAsync(
        this ISignalBotClient client,
        string recipient,
        int timestamp,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new RemoteDeleteRequest(client.Number)
        {
            Recipient = recipient,
            Timestamp = timestamp
        }, cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<InstalledStickerPack>> GetStickerPacksAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetStickerPacksRequest(client.Number),
            cancellationToken: cancellationToken);
    }

    public static async Task AddStickerPackAsync(
        this ISignalBotClient client,
        string packId,
        string packKey,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new AddStickerPackRequest(client.Number)
        {
            PackId = packId,
            PackKey = packKey
        }, cancellationToken: cancellationToken);
    }

    // Social extensions
    public static async Task<ICollection<Contact>> GetContactsAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetContactsRequest(client.Number),
            cancellationToken: cancellationToken);
    }

    public static async Task UpdateContactAsync(
        this ISignalBotClient client,
        string recipient,
        string? name = null,
        int? expirationTime = null,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new UpdateContactRequest(client.Number)
        {
            Recipient = recipient,
            Name = name,
            ExpirationTime = expirationTime
        }, cancellationToken: cancellationToken);
    }

    public static async Task SyncContactsAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new SyncContactsRequest(client.Number), cancellationToken: cancellationToken);
    }

    public static async Task<Group> GetGroupAsync(
        this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetGroupRequest(client.Number, groupId),
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupAsync(
        this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemoveGroupRequest(client.Number, groupId),
            cancellationToken: cancellationToken);
    }

    public static async Task AddGroupAdminAsync(
        this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new AddGroupAdminRequest(client.Number, groupId) { Admins = admins },
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupAdminAsync(
        this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemoveGroupAdminRequest(client.Number, groupId) { Admins = admins },
            cancellationToken: cancellationToken);
    }

    public static async Task AddGroupMemberAsync(
        this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new AddGroupMemberRequest(client.Number, groupId) { Members = members },
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupMemberAsync(
        this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new RemoveGroupMemberRequest(client.Number, groupId) { Members = members },
            cancellationToken: cancellationToken);
    }

    public static async Task QuitGroupAsync(
        this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new QuitGroupRequest(client.Number, groupId),
            cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<Identity>> GetIdentitiesAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetIdentitiesRequest(client.Number),
            cancellationToken: cancellationToken);
    }

    public static async Task TrustIdentityAsync(
        this ISignalBotClient client,
        string verifiedNumber,
        bool? trustAllKnownKeys = null,
        string? verifiedSafetyNumber = null,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new TrustIdentityRequest(client.Number, verifiedNumber)
        {
            TrustAllKnownKeys = trustAllKnownKeys,
            VerifiedSafetyNumber = verifiedSafetyNumber
        }, cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<SearchResponse>> SearchNumbersAsync(
        this ISignalBotClient client,
        IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new SearchNumbersRequest(client.Number)
        {
            Numbers = numbers as ICollection<string> ?? new List<string>(numbers)
        }, cancellationToken: cancellationToken);
    }

    // Configuration extensions
    public static async Task<Configuration> GetConfigurationAsync(
        this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetConfigurationRequest(), cancellationToken: cancellationToken);
    }

    public static async Task SetConfigurationAsync(
        this ISignalBotClient client,
        string logging,
        CancellationToken cancellationToken = default)
    {
        await client.SendRequestAsync(new SetConfigurationRequest { Logging = logging },
            cancellationToken: cancellationToken);
    }
}