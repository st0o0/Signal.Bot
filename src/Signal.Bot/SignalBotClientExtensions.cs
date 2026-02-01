using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot;

// ReSharper disable once ConvertToExtensionBlock
public static partial class SignalBotClientExtensions
{
    public static async Task<Acknowledged> SendMessageAsync(this ISignalBotClient client,
        string recipient,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        var builder = new Action<SendMessageRequestBuilder>(builder =>
            builder
                .WithRecipient(recipient)
                .WithMessage(message));
        return await client.SendMessageAsync(builder, cancellationToken);
    }

    public static async Task<Acknowledged> SendMessageAsync(this ISignalBotClient client,
        Action<SendMessageRequestBuilder> messageBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = new SendMessageRequestBuilder();
        messageBuilder(builder);
        return await client.SendRequestAsync(builder.Build(), cancellationToken: cancellationToken);
    }

    public static async Task<About> GetAboutAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAboutRequest(), cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<string>> GetAccountsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task<ICollection<Group>> GetGroupsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task RegisterNumberAsync(this ISignalBotClient client,
        string? captcha = null,
        bool? useVoice = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RegisterNumberRequest(client.Number)
        {
            Captcha = captcha,
            UseVoice = useVoice
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<string> VerifyNumberAsync(this ISignalBotClient client,
        string token,
        string? pin = null,
        CancellationToken cancellationToken = default)
    {
        var request = new VerifyNumberRequest(client.Number, token)
        {
            Pin = pin
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task UpdateProfileAsync(this ISignalBotClient client,
        string? name = null,
        string? about = null,
        string? base64Avatar = null,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateProfileRequest(client.Number)
        {
            Name = name,
            About = about,
            Base64Avatar = base64Avatar
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task SetTypingIndicatorAsync(this ISignalBotClient client,
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

    public static async Task SetPinAsync(this ISignalBotClient client, string pin,
        CancellationToken cancellationToken = default)
    {
        var request = new SetPinRequest(client.Number) { Pin = pin };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemovePinAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new RemovePinRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RateLimitChallengeAsync(this ISignalBotClient client,
        string challengeToken,
        string captcha,
        CancellationToken cancellationToken = default)
    {
        var request = new RateLimitChallengeRequest(client.Number)
        {
            ChallengeToken = challengeToken,
            Captcha = captcha
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task UpdateAccountSettingsAsync(this ISignalBotClient client,
        bool discoverableByNumber = true,
        bool shareNumberWithContacts = true,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateAccountSettingsRequest(client.Number)
        {
            DiscoverableByNumber = discoverableByNumber,
            ShareNumberWithContacts = shareNumberWithContacts
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<SetUsername> SetUsernameAsync(this ISignalBotClient client, string username,
        CancellationToken cancellationToken = default)
    {
        var request = new SetUsernameRequest(client.Number) { Username = username };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveUsernameAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveUsernameRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Device[]> GetDevicesAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetDevicesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task AddDeviceAsync(this ISignalBotClient client, string uri,
        CancellationToken cancellationToken = default)
    {
        var request = new AddDeviceRequest(client.Number) { Uri = uri };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task UnregisterDeviceAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new UnregisterDeviceRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<string[]> GetAttachmentsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAttachmentsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task<byte[]> GetAttachmentAsync(this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentId);
        var request = new GetAttachmentRequest(attachmentId);
        var result = await client.SendAsync(request, cancellationToken: cancellationToken);
        if (!result.IsSuccessStatusCode) return [];
        return await result.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public static async Task<Stream> GetAttachmentStreamAsync(this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentId);
        var request = new GetAttachmentRequest(attachmentId);
        var result = await client.SendAsync(request, cancellationToken: cancellationToken);
        return await result.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static async Task RemoveAttachmentAsync(this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveAttachmentRequest(attachmentId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task AddReactionAsync(this ISignalBotClient client,
        string reaction,
        string recipient,
        string targetAuthor,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var request = new AddReactionRequest(client.Number)
        {
            Reaction = reaction,
            Recipient = recipient,
            TargetAuthor = targetAuthor,
            Timestamp = timestamp ?? DateTime.UtcNow
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<string> RemoveReactionAsync(this ISignalBotClient client,
        string emoji,
        string recipient,
        string targetAuthor,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveReactionRequest(client.Number)
        {
            Emoji = emoji,
            Recipient = recipient,
            TargetAuthor = targetAuthor,
            Timestamp = timestamp ?? DateTime.UtcNow
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Acknowledged> RemoteDeleteAsync(this ISignalBotClient client,
        string recipient,
        int timestamp,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoteDeleteRequest(client.Number)
        {
            Recipient = recipient,
            Timestamp = timestamp
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<StickerPack[]> GetStickerPacksAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetStickerPacksRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task AddStickerPackAsync(this ISignalBotClient client,
        string packId,
        string packKey,
        CancellationToken cancellationToken = default)
    {
        var request = new AddStickerPackRequest(client.Number)
        {
            PackId = packId,
            PackKey = packKey
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Contact[]> GetContactsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetContactsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task UpdateContactAsync(this ISignalBotClient client,
        string recipient,
        string? name = null,
        int? expirationTimeInSeconds = null,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateContactRequest(client.Number)
        {
            Recipient = recipient,
            Name = name,
            ExpirationTimeInSeconds = expirationTimeInSeconds
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task SyncContactsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new SyncContactsRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Group> GetGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupRequest(client.Number, groupId);
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task AddGroupAdminAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupAdminRequest(client.Number, groupId) { Admins = admins };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupAdminAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupAdminRequest(client.Number, groupId) { Admins = admins };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task AddGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupMemberRequest(client.Number, groupId) { Members = members };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupMemberRequest(client.Number, groupId) { Members = members };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task QuitGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new QuitGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Identity[]> GetIdentitiesAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetIdentitiesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task TrustIdentityAsync(this ISignalBotClient client,
        string verifiedNumber,
        bool? trustAllKnownKeys = null,
        string? verifiedSafetyNumber = null,
        CancellationToken cancellationToken = default)
    {
        var request = new TrustIdentityRequest(client.Number, verifiedNumber)
        {
            TrustAllKnownKeys = trustAllKnownKeys,
            VerifiedSafetyNumber = verifiedSafetyNumber
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<ICollection<Search>> SearchNumbersAsync(this ISignalBotClient client,
        IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        var request = new SearchNumbersRequest(client.Number)
        {
            Numbers = numbers as ICollection<string> ?? new List<string>(numbers)
        };
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task<Configuration> GetConfigurationAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetConfigurationRequest();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task SetConfigurationAsync(this ISignalBotClient client,
        string logging,
        CancellationToken cancellationToken = default)
    {
        var request = new SetConfigurationRequest { Logging = logging };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }
}