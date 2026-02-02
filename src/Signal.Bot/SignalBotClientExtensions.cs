using Signal.Bot.Internal;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot;

public static partial class SignalBotClientExtensions
{
    #region General

    public static async Task<About> GetAboutAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAboutRequest(), cancellationToken: cancellationToken);
    }

    public static async Task<Configuration> GetConfigurationAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetConfigurationRequest();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task SetConfigurationAsync(this ISignalBotClient client,
        string level,
        CancellationToken cancellationToken = default)
    {
        var request = new SetConfigurationRequest { Logging = new Logging { Level = level } };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    // TODO: Implement
    // GET
    // /v1/configuration/{number}/settings
    //    List account specific settings.
    //    POST
    // /v1/configuration/{number}/settings
    //    Set account specific settings.

    #endregion

    #region Devices

    public static async Task<ICollection<Device>> GetDevicesAsync(this ISignalBotClient client,
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

    // TODO: Implement
    // public static async Task DeleteLocalDataAsync(this ISignalBotClient client,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new DeleteLocalDataRequest(client.Number);
    //     await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    // }

    // TODO: Implement
    // public static async Task RemoveLinkedDeviceAsync(this ISignalBotClient client,
    //     string deviceId,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new RemoveLinkedDeviceRequest(client.Number, deviceId);
    //     await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    // }

    public static async Task<string> GetQrCodeLinkAsync(this ISignalBotClient client,
        string deviceName,
        int qrCodeVersion = 10,
        CancellationToken cancellationToken = default)
    {
        var request = new GetQrCodeLinkRequest
        {
            DeviceName = deviceName,
            QrCodeVersion = qrCodeVersion
        };
        var response = await client.SendAsync(request, cancellationToken: cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public static async Task<RawDeviceLink> GetRawDeviceLinkAsync(this ISignalBotClient client,
        string deviceName,
        CancellationToken cancellationToken = default)
    {
        var request = new GetRawDeviceLinkRequest
        {
            DeviceName = deviceName
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
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

    public static async Task UnregisterDeviceAsync(this ISignalBotClient client,
        bool deleteAccount = false,
        bool deleteLocalData = false,
        CancellationToken cancellationToken = default)
    {
        var request = new UnregisterDeviceRequest(client.Number)
        {
            DeleteAccount = deleteAccount,
            DeleteLocalData = deleteLocalData
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Accounts

    public static async Task<ICollection<string>> GetAccountsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
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
        bool shareNumber = true,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateAccountSettingsRequest(client.Number)
        {
            DiscoverableByNumber = discoverableByNumber,
            ShareNumber = shareNumber
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

    #endregion

    #region Groups

    public static async Task<ICollection<Group>> GetGroupsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    public static async Task<Group> CreateGroupAsync(this ISignalBotClient client,
        Action<CreateGroupBuilder> createGroupBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = new CreateGroupBuilder(client.Number);
        createGroupBuilder.Invoke(builder);
        var request = builder.Build();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task<Group> GetGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupRequest(client.Number, groupId);
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task UpdateGroupAsync(this ISignalBotClient client,
        string groupId,
        Action<UpdateGroupBuilder> updateGroupBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = new UpdateGroupBuilder(client.Number, groupId);
        updateGroupBuilder.Invoke(builder);
        var request = builder.Build();
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
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
        IEnumerable<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupAdminRequest(client.Number, groupId) { Admins = admins.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupAdminAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupAdminRequest(client.Number, groupId) { Admins = admins.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    // TODO: Implement
    // public static async Task<byte[]> GetGroupAvatarAsync(this ISignalBotClient client,
    //     string groupId,
    //     CancellationToken cancellationToken = default)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
    //     var request = new GetGroupAvatarRequest(groupId);
    //     var result = await client.SendAsync(request, cancellationToken: cancellationToken);
    //     if (!result.IsSuccessStatusCode) return [];
    //     return await result.Content.ReadAsByteArrayAsync(cancellationToken);
    // }

    // TODO: Implement
    // public static async Task<Stream> GetGroupAvatarStreamAsync(this ISignalBotClient client,
    //     string groupId,
    //     CancellationToken cancellationToken = default)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
    //     var request = new GetGroupAvatarRequest(groupId);
    //     var result = await client.SendAsync(request, cancellationToken: cancellationToken);
    //     return await result.Content.ReadAsStreamAsync(cancellationToken);
    // }

    public static async Task BlockGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new BlockGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task JoinGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new JoinGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task AddGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupMemberRequest(client.Number, groupId) { Members = members.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task RemoveGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupMemberRequest(client.Number, groupId) { Members = members.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task QuitGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new QuitGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Messages

    // TODO: Implement
    // public static async Task<ICollection<ReceivedMessage>> ReceiveMessagesAsync(this ISignalBotClient client,
    //     int? timeout = null,
    //     int? maxMessages = null,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new ReceiveMessagesRequest(client.Number)
    //     {
    //         Timeout = timeout,
    //         MaxMessages = maxMessages
    //     };
    //     var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    //     return result?.ToArray() ?? [];
    // }

    public static async Task<Acknowledged> RemoteDeleteAsync(this ISignalBotClient client,
        string recipient,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoteDeleteRequest(client.Number)
        {
            Recipient = recipient,
            Timestamp = timestamp ?? DateTime.UtcNow
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    public static async Task SetTypingIndicatorAsync(this ISignalBotClient client,
        string? recipient = null,
        string? groupId = null,
        bool isTyping = true,
        CancellationToken cancellationToken = default)
    {
        var typing = new AddTypingIndicatorRequest(client.Number)
        {
            Recipient = recipient
        };
        var resetTyping = new RemoveTypingIndicatorRequest(client.Number)
        {
            Recipient = recipient
        };
        await client.SendRequestAsync(isTyping ? typing : resetTyping, cancellationToken: cancellationToken);
    }

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

    #endregion

    #region Attachments

    public static async Task<ICollection<string>> GetAttachmentsAsync(this ISignalBotClient client,
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

    #endregion

    #region Profiles

    public static async Task UpdateProfileAsync(this ISignalBotClient client,
        string? name = null,
        string? about = null,
        byte[]? avatar = null,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateProfileRequest(client.Number)
        {
            Name = name,
            About = about,
            Avatar = Base64String.FromBytes(avatar ?? []),
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Identities

    public static async Task<ICollection<Identity>> GetIdentitiesAsync(this ISignalBotClient client,
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

    #endregion

    #region Reactions

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
        string reaction,
        string recipient,
        string targetAuthor,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveReactionRequest(client.Number)
        {
            Reaction = reaction,
            Recipient = recipient,
            TargetAuthor = targetAuthor,
            Timestamp = timestamp ?? DateTime.UtcNow
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Receipts

    public static async Task SendReceiptAsync(this ISignalBotClient client,
        string recipient,
        DateTime? timestamp = null,
        ReceiptType receiptType = ReceiptType.Read,
        CancellationToken cancellationToken = default)
    {
        var request = new SendReceiptsRequest(client.Number)
        {
            Recipient = recipient,
            Timestamp = timestamp ?? DateTime.UtcNow,
            ReceiptType = receiptType
        };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Search

    public static async Task<ICollection<Search>> SearchNumbersAsync(this ISignalBotClient client,
        IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        var registry = new QueryParameterRegistry();
        registry.AddRange("number", numbers);
        var request = new SearchNumbersRequest(client.Number);
        var result = await client.SendRequestAsync(request, registry, cancellationToken);
        return result?.ToArray() ?? [];
    }

    #endregion

    #region Sticker Packs

    public static async Task<ICollection<StickerPack>> GetStickerPacksAsync(this ISignalBotClient client,
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

    #endregion

    #region Contacts

    public static async Task<ICollection<Contact>> GetContactsAsync(this ISignalBotClient client,
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

    public static async Task<Contact> GetContactAsync(this ISignalBotClient client,
        string contactId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetContactRequest(client.Number, contactId);
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    // TODO: Implement
    // public static async Task<byte[]> GetContactAvatarAsync(this ISignalBotClient client,
    //     string contactId,
    //     CancellationToken cancellationToken = default)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(contactId);
    //     var request = new GetContactAvatarRequest(contactId);
    //     var result = await client.SendAsync(request, cancellationToken: cancellationToken);
    //     if (!result.IsSuccessStatusCode) return [];
    //     return await result.Content.ReadAsByteArrayAsync(cancellationToken);
    // }

    // TODO: Implement
    // public static async Task<Stream> GetContactAvatarStreamAsync(this ISignalBotClient client,
    //     string contactId,
    //     CancellationToken cancellationToken = default)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(contactId);
    //     var request = new GetContactAvatarRequest(contactId);
    //     var result = await client.SendAsync(request, cancellationToken: cancellationToken);
    //     return await result.Content.ReadAsStreamAsync(cancellationToken);
    // }

    #endregion

    #region Polls

    // TODO: Implement
    // public static async Task<Acknowledged> CreatePollAsync(this ISignalBotClient client,
    //     string recipient,
    //     string question,
    //     ICollection<string> options,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new CreatePollRequest(client.Number)
    //     {
    //         Recipient = recipient,
    //         Question = question,
    //         Options = options
    //     };
    //     return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    // }

    // TODO: Implement
    // public static async Task ClosePollAsync(this ISignalBotClient client,
    //     string recipient,
    //     DateTime? pollTimestamp = null,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new ClosePollRequest(client.Number)
    //     {
    //         Recipient = recipient,
    //         PollTimestamp = pollTimestamp
    //     };
    //     return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    // }

    // TODO: Implement
    // public static async Task AnswerPollAsync(this ISignalBotClient client,
    //     string recipient,
    //     long timestamp,
    //     ICollection<int> selectedOptions,
    //     CancellationToken cancellationToken = default)
    // {
    //     var request = new AnswerPollRequest(client.Number)
    //     {
    //         Recipient = recipient,
    //         Timestamp = timestamp,
    //         SelectedOptions = selectedOptions
    //     };
    //     return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    // }

    #endregion
}