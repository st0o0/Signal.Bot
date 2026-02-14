using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot;

/// <summary>
/// Provides extension methods for <see cref="ISignalBotClient"/> to simplify common Signal Bot API operations.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Sends a simple text message to a recipient.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="recipient">The phone number or group ID of the recipient.</param>
    /// <param name="message">The text message to send.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="Acknowledged"/> response confirming the message was sent.</returns>
    /// <exception cref="ArgumentException">Thrown when recipient or message is null or whitespace.</exception>
    public static async Task<Acknowledged> SendMessageAsync(this ISignalBotClient client, string recipient,
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

    /// <summary>
    /// Sends a message using a fluent builder for advanced configuration.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="messageBuilder">Action to configure the message using <see cref="SendMessageRequestBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="Acknowledged"/> response confirming the message was sent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when messageBuilder is null.</exception>
    public static async Task<Acknowledged> SendMessageAsync(this ISignalBotClient client, Action<SendMessageRequestBuilder> messageBuilder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messageBuilder);
        var builder = new SendMessageRequestBuilder();
        messageBuilder.Invoke(builder);
        return await client.SendRequestAsync(builder.Build(), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves information about the Signal Bot API server.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns><see cref="About"/> information including version and build details.</returns>
    public static async Task<About> GetAboutAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAboutRequest(), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a list of all registered Signal accounts on this server.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Collection of phone numbers for registered accounts.</returns>
    public static async Task<ICollection<string>> GetAccountsAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetAccountsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets all groups that the current account is a member of.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Collection of <see cref="Group"/> objects.</returns>
    public static async Task<ICollection<Group>> GetGroupsAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetGroupsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Registers a new phone number with Signal.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="captcha">Optional captcha token for verification.</param>
    /// <param name="useVoice">If true, requests verification code via voice call instead of SMS.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RegisterNumberAsync(this ISignalBotClient client, string? captcha = null,
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

    /// <summary>
    /// Verifies a phone number using the verification code received during registration.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="token">The verification code received via SMS or voice call.</param>
    /// <param name="pin">Optional registration lock PIN if previously set.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The verification status or account identifier.</returns>
    public static async Task<string> VerifyNumberAsync(this ISignalBotClient client, string token,
        string? pin = null,
        CancellationToken cancellationToken = default)
    {
        var request = new VerifyNumberRequest(client.Number, token)
        {
            Pin = pin
        };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the Signal profile information for the current account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="name">The display name to set.</param>
    /// <param name="about">The about/status text to set.</param>
    /// <param name="base64Avatar">Base64-encoded avatar image.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task UpdateProfileAsync(this ISignalBotClient client, string? name = null,
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

    /// <summary>
    /// Sets or clears the typing indicator in a conversation.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="recipient">The phone number of the recipient (for direct messages).</param>
    /// <param name="groupId">The group ID (for group messages).</param>
    /// <param name="isTyping">True to show typing indicator, false to hide it.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task SetTypingIndicatorAsync(this ISignalBotClient client, string? recipient = null,
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

    /// <summary>
    /// Sets a registration lock PIN for the account to prevent unauthorized re-registration.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="pin">The PIN to set (must be numeric).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task SetPinAsync(this ISignalBotClient client, string pin,
        CancellationToken cancellationToken = default)
    {
        var request = new SetPinRequest(client.Number) { Pin = pin };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes the registration lock PIN from the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemovePinAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new RemovePinRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Responds to a rate limit challenge using a captcha token.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="challengeToken">The challenge token received from Signal.</param>
    /// <param name="captcha">The captcha solution token.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RateLimitChallengeAsync(this ISignalBotClient client, string challengeToken,
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

    /// <summary>
    /// Updates account privacy settings.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="discoverableByNumber">If true, allows others to find you by phone number.</param>
    /// <param name="shareNumberWithContacts">If true, shares your phone number with contacts.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task UpdateAccountSettingsAsync(this ISignalBotClient client, bool discoverableByNumber = true,
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

    /// <summary>
    /// Sets a Signal username for the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="username">The username to set (must be unique).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns><see cref="SetUsername"/> information about the set username.</returns>
    public static async Task<SetUsername> SetUsernameAsync(this ISignalBotClient client, string username,
        CancellationToken cancellationToken = default)
    {
        var request = new SetUsernameRequest(client.Number) { Username = username };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes the Signal username from the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemoveUsernameAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new RemoveUsernameRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a list of all linked devices for the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Array of <see cref="Device"/> objects.</returns>
    public static async Task<Device[]> GetDevicesAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetDevicesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Links a new device to the account using a device linking URI.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="uri">The device linking URI (QR code data).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task AddDeviceAsync(this ISignalBotClient client, string uri,
        CancellationToken cancellationToken = default)
    {
        var request = new AddDeviceRequest(client.Number) { Uri = uri };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Unregisters the current device from the Signal account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task UnregisterDeviceAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new UnregisterDeviceRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a list of all stored attachment IDs.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Array of attachment identifiers.</returns>
    public static async Task<string[]> GetAttachmentsAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetAttachmentsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Downloads an attachment by ID as a byte array.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The attachment data as a byte array, or empty array if download fails.</returns>
    /// <exception cref="ArgumentException">Thrown when attachmentId is null or whitespace.</exception>
    public static async Task<byte[]> GetAttachmentAsync(this ISignalBotClient client, string attachmentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentId);
        var request = new GetAttachmentRequest(attachmentId);
        var result = await client.SendAsync(request, cancellationToken: cancellationToken);
        if (!result.IsSuccessStatusCode) return [];
        return await result.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Downloads an attachment by ID as a stream for efficient large file handling.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Stream"/> containing the attachment data.</returns>
    /// <exception cref="ArgumentException">Thrown when attachmentId is null or whitespace.</exception>
    public static async Task<Stream> GetAttachmentStreamAsync(this ISignalBotClient client, string attachmentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentId);
        var request = new GetAttachmentRequest(attachmentId);
        var result = await client.SendAsync(request, cancellationToken: cancellationToken);
        return await result.Content.ReadAsStreamAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a stored attachment from the server.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment to remove.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemoveAttachmentAsync(this ISignalBotClient client, string attachmentId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveAttachmentRequest(attachmentId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an emoji reaction to a message.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="reaction">The emoji to use as a reaction.</param>
    /// <param name="recipient">The phone number or group ID where the message was sent.</param>
    /// <param name="targetAuthor">The author of the message being reacted to.</param>
    /// <param name="timestamp">Optional timestamp of the target message (defaults to current UTC time).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task AddReactionAsync(this ISignalBotClient client, string reaction,
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

    /// <summary>
    /// Removes a previously added emoji reaction from a message.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="emoji">The emoji reaction to remove.</param>
    /// <param name="recipient">The phone number or group ID where the message was sent.</param>
    /// <param name="targetAuthor">The author of the message with the reaction.</param>
    /// <param name="timestamp">Optional timestamp of the target message (defaults to current UTC time).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Confirmation string or status of the removal.</returns>
    public static async Task<string> RemoveReactionAsync(this ISignalBotClient client, string emoji,
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

    /// <summary>
    /// Remotely deletes a sent message for all recipients.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="recipient">The phone number or group ID where the message was sent.</param>
    /// <param name="timestamp">The timestamp of the message to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="Acknowledged"/> response confirming the deletion was sent.</returns>
    public static async Task<Acknowledged> RemoteDeleteAsync(this ISignalBotClient client, string recipient,
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

    /// <summary>
    /// Gets all installed sticker packs for the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Array of <see cref="StickerPack"/> objects.</returns>
    public static async Task<StickerPack[]> GetStickerPacksAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetStickerPacksRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Installs a sticker pack using its pack ID and decryption key.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="packId">The unique identifier of the sticker pack.</param>
    /// <param name="packKey">The encryption key for the sticker pack.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task AddStickerPackAsync(this ISignalBotClient client, string packId,
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

    /// <summary>
    /// Gets all contacts from the account's contact list.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Array of <see cref="Contact"/> objects.</returns>
    public static async Task<Contact[]> GetContactsAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetContactsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Updates contact information for a specific recipient.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="recipient">The phone number of the contact to update.</param>
    /// <param name="name">Optional display name for the contact.</param>
    /// <param name="expirationTimeInSeconds">Optional disappearing message timer in seconds.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task UpdateContactAsync(this ISignalBotClient client, string recipient,
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

    /// <summary>
    /// Synchronizes contacts with the Signal server.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task SyncContactsAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new SyncContactsRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets detailed information about a specific group.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The <see cref="Group"/> details.</returns>
    public static async Task<Group> GetGroupAsync(this ISignalBotClient client, string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupRequest(client.Number, groupId);
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes a group (must be group admin).
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group to remove.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemoveGroupAsync(this ISignalBotClient client, string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Promotes members to group administrators.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="admins">Collection of phone numbers to promote to admin.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task AddGroupAdminAsync(this ISignalBotClient client, string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupAdminRequest(client.Number, groupId) { Admins = admins };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Demotes administrators to regular group members.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="admins">Collection of phone numbers to demote from admin.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemoveGroupAdminAsync(this ISignalBotClient client, string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupAdminRequest(client.Number, groupId) { Admins = admins };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds new members to a group.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="members">Collection of phone numbers to add to the group.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task AddGroupMemberAsync(this ISignalBotClient client, string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupMemberRequest(client.Number, groupId) { Members = members };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes members from a group (requires admin privileges).
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="members">Collection of phone numbers to remove from the group.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task RemoveGroupMemberAsync(this ISignalBotClient client, string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupMemberRequest(client.Number, groupId) { Members = members };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Leaves a group as the current user.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="groupId">The unique identifier of the group to leave.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task QuitGroupAsync(this ISignalBotClient client, string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new QuitGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets all known identity keys (safety numbers) for the account.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Array of <see cref="Identity"/> information.</returns>
    public static async Task<Identity[]> GetIdentitiesAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetIdentitiesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Marks an identity (safety number) as trusted after verification.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="verifiedNumber">The phone number of the contact to trust.</param>
    /// <param name="trustAllKnownKeys">If true, trusts all known keys for this contact.</param>
    /// <param name="verifiedSafetyNumber">Optional safety number string to verify.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task TrustIdentityAsync(this ISignalBotClient client, string verifiedNumber,
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

    /// <summary>
    /// Searches for Signal accounts by phone numbers to check if they are registered.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="numbers">Collection of phone numbers to search for.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Collection of <see cref="Search"/> results indicating registration status.</returns>
    public static async Task<ICollection<Search>> SearchNumbersAsync(this ISignalBotClient client, IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        var request = new SearchNumbersRequest(client.Number)
        {
            Numbers = numbers as ICollection<string> ?? new List<string>(numbers)
        };
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets the current Signal Bot API server configuration.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The <see cref="Configuration"/> settings.</returns>
    public static async Task<Configuration> GetConfigurationAsync(this ISignalBotClient client, CancellationToken cancellationToken = default)
    {
        var request = new GetConfigurationRequest();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the Signal Bot API server configuration (requires admin access).
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="logging">The logging configuration string.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    public static async Task SetConfigurationAsync(this ISignalBotClient client, string logging,
        CancellationToken cancellationToken = default)
    {
        var request = new SetConfigurationRequest { Logging = logging };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }
}