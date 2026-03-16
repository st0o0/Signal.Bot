using Signal.Bot.Internal;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot;

/// <summary>
/// Provides extension methods for <see cref="ISignalBotClient"/> to simplify common Signal Bot API operations.
/// </summary>
public static class Extensions
{
    #region General

    /// <summary>
    /// Retrieves general information about the Signal Bot API service.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>An <see cref="About"/> object containing service information.</returns>
    public static async Task<About> GetAboutAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        return await client.SendRequestAsync(new GetAboutRequest(), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves the current configuration settings of the Signal Bot API.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Configuration"/> object containing the current settings.</returns>
    public static async Task<Configuration> GetConfigurationAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetConfigurationRequest();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the logging level configuration of the Signal Bot API.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="level">The logging level to set (e.g., "debug", "info", "warn", "error").</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SetConfigurationAsync(this ISignalBotClient client,
        string level,
        CancellationToken cancellationToken = default)
    {
        var request = new SetConfigurationRequest { Logging = new Logging { Level = level } };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Devices

    /// <summary>
    /// Retrieves all linked devices associated with the bot's Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="Device"/> objects representing linked devices.</returns>
    public static async Task<ICollection<Device>> GetDevicesAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetDevicesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Links a new device to the bot's Signal account using a provisioning URI.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="uri">The device provisioning URI obtained from the Signal app.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddDeviceAsync(this ISignalBotClient client, string uri,
        CancellationToken cancellationToken = default)
    {
        var request = new AddDeviceRequest(client.Number) { Uri = uri };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generates a QR code link for linking a new device to the Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="deviceName">The name to assign to the new device.</param>
    /// <param name="qrCodeVersion">The QR code version to generate. Default is 10.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A string containing the QR code data or link.</returns>
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

    /// <summary>
    /// Retrieves the raw device linking information for provisioning a new device.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="deviceName">The name to assign to the new device.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="RawDeviceLink"/> object containing provisioning details.</returns>
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

    /// <summary>
    /// Initiates the registration process for a phone number with Signal.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="captcha">Optional CAPTCHA token if required by Signal.</param>
    /// <param name="useVoice">If true, requests a voice call instead of SMS for verification. Default is false.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Verifies a phone number registration using the verification code received via SMS or voice call.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="token">The verification code received from Signal.</param>
    /// <param name="pin">Optional registration PIN if Signal account has PIN enabled.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A string containing the verification result or session information.</returns>
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

    /// <summary>
    /// Unregisters the current device from Signal and optionally deletes the account and local data.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="deleteAccount">If true, permanently deletes the Signal account. Default is false.</param>
    /// <param name="deleteLocalData">If true, deletes local data stored by the bot. Default is false.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Retrieves all registered Signal accounts managed by this bot instance.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of phone numbers representing registered accounts.</returns>
    public static async Task<ICollection<string>> GetAccountsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Sets or updates the registration lock PIN for the Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="pin">The PIN to set (typically 4-8 digits).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SetPinAsync(this ISignalBotClient client, string pin,
        CancellationToken cancellationToken = default)
    {
        var request = new SetPinRequest(client.Number) { Pin = pin };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes the registration lock PIN from the Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemovePinAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new RemovePinRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Completes a rate limit challenge when Signal requires additional verification.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="challengeToken">The challenge token provided by Signal.</param>
    /// <param name="captcha">The solved CAPTCHA token.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Updates the account privacy and discoverability settings.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="discoverableByNumber">If true, allows others to find this account by phone number. Default is true.</param>
    /// <param name="shareNumber">If true, shares the phone number with contacts. Default is true.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Sets a unique username for the Signal account, allowing others to find you without sharing your phone number.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="username">The username to set (must be unique across Signal).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="SetUsername"/> object containing the username confirmation details.</returns>
    public static async Task<SetUsername> SetUsernameAsync(this ISignalBotClient client, string username,
        CancellationToken cancellationToken = default)
    {
        var request = new SetUsernameRequest(client.Number) { Username = username };
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes the username from the Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemoveUsernameAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveUsernameRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Groups

    /// <summary>
    /// Retrieves all Signal groups that the bot is a member of.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="useOnlyIdAsIdentifier">Use UUIDs instead of phone numbers as identifier for (pending|requesting) members</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="Group"/> objects.</returns>
    public static async Task<ICollection<Group>> GetGroupsAsync(this ISignalBotClient client,
        bool useOnlyIdAsIdentifier = false,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupsRequest(client.Number);
        var queryParameter = new QueryParameterRegistry();
        queryParameter.Add("use_only_uuid_as_identifier", useOnlyIdAsIdentifier);
        var result = await client.SendRequestAsync(request, queryParameter, cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Creates a new Signal group with specified settings and initial members.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="createGroupBuilder">An action to configure the group creation using <see cref="CreateGroupBuilder"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Group"/> object representing the newly created group.</returns>
    public static async Task<Group> CreateGroupAsync(this ISignalBotClient client,
        Action<CreateGroupBuilder> createGroupBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = new CreateGroupBuilder(client.Number);
        createGroupBuilder.Invoke(builder);
        var request = builder.Build();
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a specific Signal group.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="useOnlyIdAsIdentifier">Use UUIDs instead of phone numbers as identifier for (pending|requesting) members</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Group"/> object containing the group details.</returns>
    public static async Task<Group> GetGroupAsync(this ISignalBotClient client,
        string groupId,
        bool useOnlyIdAsIdentifier = false,
        CancellationToken cancellationToken = default)
    {
        var request = new GetGroupRequest(client.Number, groupId);
        var queryParameter = new QueryParameterRegistry();
        queryParameter.Add("use_only_uuid_as_identifier", useOnlyIdAsIdentifier);
        return await client.SendRequestAsync(request, queryParameter, cancellationToken);
    }

    /// <summary>
    /// Updates the settings of an existing Signal group.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group to update.</param>
    /// <param name="updateGroupBuilder">An action to configure the group updates using <see cref="UpdateGroupBuilder"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Deletes a Signal group. Only group administrators can perform this action.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemoveGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Promotes one or more group members to administrator status.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="admins">The phone numbers of members to promote to admin.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddGroupAdminAsync(this ISignalBotClient client,
        string groupId,
        IEnumerable<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupAdminRequest(client.Number, groupId) { Admins = admins.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes administrator privileges from one or more group admins.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="admins">The phone numbers of admins to demote.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemoveGroupAdminAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> admins,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupAdminRequest(client.Number, groupId) { Admins = admins.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Blocks all messages from a specific Signal group.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group to block.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task BlockGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new BlockGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Joins an existing Signal group using a group link or invitation.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group to join.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task JoinGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new JoinGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds one or more members to a Signal group.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="members">The phone numbers of members to add to the group.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new AddGroupMemberRequest(client.Number, groupId) { Members = members.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes one or more members from a Signal group.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="members">The phone numbers of members to remove from the group.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemoveGroupMemberAsync(this ISignalBotClient client,
        string groupId,
        ICollection<string> members,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupMemberRequest(client.Number, groupId) { Members = members.ToArray() };
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Leaves a Signal group, removing the bot from the member list.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="groupId">The unique identifier of the group to leave.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task QuitGroupAsync(this ISignalBotClient client,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new QuitGroupRequest(client.Number, groupId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Messages

    /// <summary>
    /// Remotely deletes a message from the conversation for all participants.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="recipient">The phone number or group ID where the message should be deleted.</param>
    /// <param name="timestamp">The timestamp of the message to delete. If null, uses current UTC time.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>An <see cref="RemoteDelete"/> response confirming the deletion request.</returns>
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

    /// <summary>
    /// Sends or removes a typing indicator to show that the bot is composing a message.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="recipient">The phone number of the recipient to send the typing indicator to.</param>
    /// <param name="groupId">The group ID if sending the typing indicator to a group.</param>
    /// <param name="isTyping">If true, shows typing indicator; if false, removes it. Default is true.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Sends a simple text message to a recipient.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="recipient">The phone number or group ID of the recipient.</param>
    /// <param name="message">The text message to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>An <see cref="RemoteDelete"/> response confirming the message was sent.</returns>
    /// <exception cref="ArgumentException">Thrown when recipient or message is null or whitespace.</exception>
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

    /// <summary>
    /// Sends a message with advanced configuration options such as attachments, mentions, and formatting.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="messageBuilder">An action to configure the message using <see cref="SendMessageRequestBuilder"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>An <see cref="Acknowledged"/> response confirming the message was sent.</returns>
    public static async Task<Acknowledged> SendMessageAsync(this ISignalBotClient client,
        Action<SendMessageRequestBuilder> messageBuilder,
        CancellationToken cancellationToken = default)
    {
        var builder = SendMessageRequestBuilder.Create(client.Number);
        messageBuilder(builder);
        return await client.SendRequestAsync(builder.Build(), cancellationToken: cancellationToken);
    }

    #endregion

    #region Attachments

    /// <summary>
    /// Retrieves a list of all attachment IDs stored by the bot.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of attachment ID strings.</returns>
    public static async Task<ICollection<string>> GetAttachmentsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAttachmentsRequest();
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Downloads an attachment as a byte array.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment to download.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A byte array containing the attachment data, or an empty array if the request fails.</returns>
    /// <exception cref="ArgumentException">Thrown when attachmentId is null or whitespace.</exception>
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

    /// <summary>
    /// Downloads an attachment as a stream for efficient memory usage with large files.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment to download.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Stream"/> containing the attachment data.</returns>
    /// <exception cref="ArgumentException">Thrown when attachmentId is null or whitespace.</exception>
    public static async Task<Stream> GetAttachmentStreamAsync(this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentId);
        var request = new GetAttachmentRequest(attachmentId);
        var result = await client.SendAsync(request, cancellationToken: cancellationToken);
        return await result.Content.ReadAsStreamAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes an attachment from the bot's storage.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="attachmentId">The unique identifier of the attachment to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RemoveAttachmentAsync(this ISignalBotClient client,
        string attachmentId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveAttachmentRequest(attachmentId);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Profiles

    /// <summary>
    /// Updates the bot's Signal profile information including name, about text, and avatar image.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="name">The display name to set for the profile.</param>
    /// <param name="about">The about/status text to set for the profile.</param>
    /// <param name="avatar">The avatar image as a byte array (JPEG or PNG format recommended).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Retrieves all known identity keys for contacts, used for verifying end-to-end encryption.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="Identity"/> objects containing identity key information.</returns>
    public static async Task<ICollection<Identity>> GetIdentitiesAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetIdentitiesRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Marks a contact's identity key as trusted after verifying their safety number.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="verifiedNumber">The phone number of the contact whose identity is being verified.</param>
    /// <param name="trustAllKnownKeys">If true, trusts all known keys for this contact. Default is null.</param>
    /// <param name="verifiedSafetyNumber">The safety number to verify, obtained through QR code or manual comparison.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Adds an emoji reaction to a specific message.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="reaction">The emoji to react with (e.g., "👍", "❤️", "😂").</param>
    /// <param name="recipient">The phone number or group ID of the conversation.</param>
    /// <param name="targetAuthor">The phone number of the message author.</param>
    /// <param name="timestamp">The timestamp of the message to react to. If null, uses current UTC time.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Removes an emoji reaction from a specific message.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="reaction">The emoji reaction to remove (e.g., "👍", "❤️", "😂").</param>
    /// <param name="recipient">The phone number or group ID of the conversation.</param>
    /// <param name="targetAuthor">The phone number of the message author.</param>
    /// <param name="timestamp">The timestamp of the message. If null, uses current UTC time.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A string containing the result of the removal operation.</returns>
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

    /// <summary>
    /// Sends a read receipt, delivery receipt, or viewed receipt for a message.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="recipient">The phone number of the message sender.</param>
    /// <param name="timestamp">The timestamp of the message. If null, uses current UTC time.</param>
    /// <param name="receiptType">The type of receipt to send (Read, Delivery, or Viewed). Default is Read.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Searches for Signal users by their phone numbers to check if they are registered on Signal.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="numbers">The phone numbers to search for (in international format).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="Search"/> objects containing registration status for each number.</returns>
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

    /// <summary>
    /// Retrieves all sticker packs installed or available to the bot.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="StickerPack"/> objects.</returns>
    public static async Task<ICollection<StickerPack>> GetStickerPacksAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetStickerPacksRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Installs a sticker pack to the bot's account using the pack ID and decryption key.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="packId">The unique identifier of the sticker pack.</param>
    /// <param name="packKey">The decryption key for the sticker pack.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Retrieves all contacts stored in the bot's Signal account.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="Contact"/> objects.</returns>
    public static async Task<ICollection<Contact>> GetContactsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new GetContactsRequest(client.Number);
        var result = await client.SendRequestAsync(request, cancellationToken: cancellationToken);
        return result?.ToArray() ?? [];
    }

    /// <summary>
    /// Updates contact information including display name and disappearing message timer.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="recipient">The phone number of the contact to update.</param>
    /// <param name="name">The display name to set for this contact.</param>
    /// <param name="expirationTimeInSeconds">The disappearing message timer in seconds (0 to disable).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Synchronizes the local contact list with Signal's servers to update profile information and registration status.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SyncContactsAsync(this ISignalBotClient client,
        CancellationToken cancellationToken = default)
    {
        var request = new SyncContactsRequest(client.Number);
        await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a specific contact.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance.</param>
    /// <param name="contactId">The phone number or unique identifier of the contact.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Contact"/> object containing the contact's details.</returns>
    public static async Task<Contact> GetContactAsync(this ISignalBotClient client,
        string contactId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetContactRequest(client.Number, contactId);
        return await client.SendRequestAsync(request, cancellationToken: cancellationToken);
    }

    #endregion
}