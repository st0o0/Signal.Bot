using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Serialization;

/// <summary>
/// Provides centralized JSON serialization configuration for the Signal Bot API client with source-generated serialization support.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class JsonBotAPI
{
    /// <summary>
    /// Gets the configured <see cref="JsonSerializerOptions"/> instance for Signal Bot API serialization.
    /// Includes snake_case naming policy, custom converters for enums and timestamps, and source-generated type information for improved performance.
    /// </summary>
    public static JsonSerializerOptions Options { get; }

    /// <summary>
    /// Initializes the static members with configured serialization options and type information resolvers.
    /// </summary>
    static JsonBotAPI()
    {
        Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            TypeInfoResolver = JsonBotSerializerContext.Default,
            Converters = { new TimestampConverter() }
        };
    }

    /// <summary>
    /// Retrieves the <see cref="JsonTypeInfo"/> for a specified type from the pre-registered type information dictionary.
    /// </summary>
    /// <param name="key">The <see cref="Type"/> for which to retrieve type information.</param>
    /// <returns>The <see cref="JsonTypeInfo"/> associated with the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no type information is registered for the specified type.</exception>
    public static JsonTypeInfo Get(Type key)
    {
        return TypeInfos.TryGetValue(key, out var typeInfo)
            ? typeInfo
            : throw new InvalidOperationException($"No JsonTypeInfo for {key.Name}");
    }

    /// <summary>
    /// Retrieves the strongly-typed <see cref="JsonTypeInfo{T}"/> for a specified type parameter.
    /// </summary>
    /// <typeparam name="T">The type for which to retrieve type information.</typeparam>
    /// <returns>The <see cref="JsonTypeInfo{T}"/> associated with the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no type information is registered for the specified type.</exception>
    public static JsonTypeInfo<T> Get<T>() => (Get(typeof(T)) as JsonTypeInfo<T>)!;

    private static readonly ImmutableDictionary<Type, JsonTypeInfo> TypeInfos = new Dictionary<Type, JsonTypeInfo>
    {
        // Response Types
        { typeof(About), JsonBotSerializerContext.Default.About },
        { typeof(Acknowledged), JsonBotSerializerContext.Default.Acknowledged },
        { typeof(Attachment), JsonBotSerializerContext.Default.Attachment },
        { typeof(Configuration), JsonBotSerializerContext.Default.Configuration },
        { typeof(Contact), JsonBotSerializerContext.Default.Contact },
        { typeof(DataMessage), JsonBotSerializerContext.Default.DataMessage },
        { typeof(Device), JsonBotSerializerContext.Default.Device },
        { typeof(Envelope), JsonBotSerializerContext.Default.Envelope },
        { typeof(ErrorResponse), JsonBotSerializerContext.Default.ErrorResponse },
        { typeof(Group), JsonBotSerializerContext.Default.Group },
        { typeof(GroupInfo), JsonBotSerializerContext.Default.GroupInfo },
        { typeof(Identity), JsonBotSerializerContext.Default.Identity },
        { typeof(LoggingConfiguration), JsonBotSerializerContext.Default.LoggingConfiguration },
        { typeof(Mention), JsonBotSerializerContext.Default.Mention },
        { typeof(Nickname), JsonBotSerializerContext.Default.Nickname },
        { typeof(Preview), JsonBotSerializerContext.Default.Preview },
        { typeof(Profile), JsonBotSerializerContext.Default.Profile },
        { typeof(ProfileCapabilities), JsonBotSerializerContext.Default.ProfileCapabilities },
        { typeof(Quote), JsonBotSerializerContext.Default.Quote },
        { typeof(RawDeviceLink), JsonBotSerializerContext.Default.RawDeviceLink },
        { typeof(Reaction), JsonBotSerializerContext.Default.Reaction },
        { typeof(ReadMessage), JsonBotSerializerContext.Default.ReadMessage },
        { typeof(ReceiptMessage), JsonBotSerializerContext.Default.ReceiptMessage },
        { typeof(ReceivedMessage), JsonBotSerializerContext.Default.ReceivedMessage },
        { typeof(Search), JsonBotSerializerContext.Default.Search },
        { typeof(SetUsername), JsonBotSerializerContext.Default.SetUsername },
        { typeof(StickerPack), JsonBotSerializerContext.Default.StickerPack },
        { typeof(SyncMessage), JsonBotSerializerContext.Default.SyncMessage },
        { typeof(TypingMessage), JsonBotSerializerContext.Default.TypingMessage },
        { typeof(CallMessage), JsonBotSerializerContext.Default.CallMessage },
        { typeof(OfferMessage), JsonBotSerializerContext.Default.OfferMessage },
        { typeof(HangupMessage), JsonBotSerializerContext.Default.HangupMessage },
        { typeof(IceUpdateMessage), JsonBotSerializerContext.Default.IceUpdateMessage },
        { typeof(PollResponse), JsonBotSerializerContext.Default.PollResponse },
        // Request Types
        { typeof(AddDeviceRequest), JsonBotSerializerContext.Default.AddDeviceRequest },
        { typeof(AddGroupAdminRequest), JsonBotSerializerContext.Default.AddGroupAdminRequest },
        { typeof(AddGroupMemberRequest), JsonBotSerializerContext.Default.AddGroupMemberRequest },
        { typeof(AddReactionRequest), JsonBotSerializerContext.Default.AddReactionRequest },
        { typeof(AddStickerPackRequest), JsonBotSerializerContext.Default.AddStickerPackRequest },
        { typeof(AddTypingIndicatorRequest), JsonBotSerializerContext.Default.AddTypingIndicatorRequest },
        { typeof(BlockGroupRequest), JsonBotSerializerContext.Default.BlockGroupRequest },
        { typeof(CreateGroupRequest), JsonBotSerializerContext.Default.CreateGroupRequest },
        { typeof(GetAboutRequest), JsonBotSerializerContext.Default.GetAboutRequest },
        { typeof(GetAccountsRequest), JsonBotSerializerContext.Default.GetAccountsRequest },
        { typeof(GetAttachmentRequest), JsonBotSerializerContext.Default.GetAttachmentRequest },
        { typeof(GetAttachmentsRequest), JsonBotSerializerContext.Default.GetAttachmentsRequest },
        { typeof(GetConfigurationRequest), JsonBotSerializerContext.Default.GetConfigurationRequest },
        { typeof(GetContactRequest), JsonBotSerializerContext.Default.GetContactRequest },
        { typeof(GetContactsRequest), JsonBotSerializerContext.Default.GetContactsRequest },
        { typeof(GetDevicesRequest), JsonBotSerializerContext.Default.GetDevicesRequest },
        { typeof(GetGroupRequest), JsonBotSerializerContext.Default.GetGroupRequest },
        { typeof(GetGroupsRequest), JsonBotSerializerContext.Default.GetGroupsRequest },
        { typeof(GetIdentitiesRequest), JsonBotSerializerContext.Default.GetIdentitiesRequest },
        { typeof(GetQrCodeLinkRequest), JsonBotSerializerContext.Default.GetQrCodeLinkRequest },
        { typeof(GetRawDeviceLinkRequest), JsonBotSerializerContext.Default.GetRawDeviceLinkRequest },
        { typeof(GetReceivedMessagesRequest), JsonBotSerializerContext.Default.GetReceivedMessagesRequest },
        { typeof(GetStickerPacksRequest), JsonBotSerializerContext.Default.GetStickerPacksRequest },
        { typeof(JoinGroupRequest), JsonBotSerializerContext.Default.JoinGroupRequest },
        { typeof(QuitGroupRequest), JsonBotSerializerContext.Default.QuitGroupRequest },
        { typeof(RateLimitChallengeRequest), JsonBotSerializerContext.Default.RateLimitChallengeRequest },
        { typeof(RegisterNumberRequest), JsonBotSerializerContext.Default.RegisterNumberRequest },
        { typeof(RemoteDeleteRequest), JsonBotSerializerContext.Default.RemoteDeleteRequest },
        { typeof(RemoveAttachmentRequest), JsonBotSerializerContext.Default.RemoveAttachmentRequest },
        { typeof(RemoveGroupAdminRequest), JsonBotSerializerContext.Default.RemoveGroupAdminRequest },
        { typeof(RemoveGroupMemberRequest), JsonBotSerializerContext.Default.RemoveGroupMemberRequest },
        { typeof(RemoveGroupRequest), JsonBotSerializerContext.Default.RemoveGroupRequest },
        { typeof(RemovePinRequest), JsonBotSerializerContext.Default.RemovePinRequest },
        { typeof(RemoveReactionRequest), JsonBotSerializerContext.Default.RemoveReactionRequest },
        { typeof(RemoveTypingIndicatorRequest), JsonBotSerializerContext.Default.RemoveTypingIndicatorRequest },
        { typeof(RemoveUsernameRequest), JsonBotSerializerContext.Default.RemoveUsernameRequest },
        { typeof(SearchNumbersRequest), JsonBotSerializerContext.Default.SearchNumbersRequest },
        { typeof(SendMessageRequest), JsonBotSerializerContext.Default.SendMessageRequest },
        { typeof(SendReceiptsRequest), JsonBotSerializerContext.Default.SendReceiptsRequest },
        { typeof(SetConfigurationRequest), JsonBotSerializerContext.Default.SetConfigurationRequest },
        { typeof(SetPinRequest), JsonBotSerializerContext.Default.SetPinRequest },
        { typeof(SetUsernameRequest), JsonBotSerializerContext.Default.SetUsernameRequest },
        { typeof(SyncContactsRequest), JsonBotSerializerContext.Default.SyncContactsRequest },
        { typeof(TrustIdentityRequest), JsonBotSerializerContext.Default.TrustIdentityRequest },
        { typeof(UnregisterDeviceRequest), JsonBotSerializerContext.Default.UnregisterDeviceRequest },
        { typeof(UpdateAccountSettingsRequest), JsonBotSerializerContext.Default.UpdateAccountSettingsRequest },
        { typeof(UpdateContactRequest), JsonBotSerializerContext.Default.UpdateContactRequest },
        { typeof(UpdateGroupRequest), JsonBotSerializerContext.Default.UpdateGroupRequest },
        { typeof(UpdateProfileRequest), JsonBotSerializerContext.Default.UpdateProfileRequest },
        { typeof(VerifyNumberRequest), JsonBotSerializerContext.Default.VerifyNumberRequest },
        { typeof(AddPollRequest), JsonBotSerializerContext.Default.AddPollRequest },
        { typeof(ClosePollRequest), JsonBotSerializerContext.Default.ClosePollRequest },
        { typeof(VotePollRequest), JsonBotSerializerContext.Default.VotePollRequest },
        
        // Collection Types
        { typeof(List<Group>), JsonBotSerializerContext.Default.ListGroup },
        { typeof(List<Attachment>), JsonBotSerializerContext.Default.ListAttachment },
        { typeof(List<Mention>), JsonBotSerializerContext.Default.ListMention },
        { typeof(List<Device>), JsonBotSerializerContext.Default.ListDevice },
        { typeof(List<Identity>), JsonBotSerializerContext.Default.ListIdentity },
        { typeof(List<Contact>), JsonBotSerializerContext.Default.ListContact },
        { typeof(List<Search>), JsonBotSerializerContext.Default.ListSearch },
        { typeof(List<StickerPack>), JsonBotSerializerContext.Default.ListStickerPack },
        { typeof(List<string>), JsonBotSerializerContext.Default.ListString },
        { typeof(string[]), JsonBotSerializerContext.Default.StringArray },
        { typeof(string), JsonBotSerializerContext.Default.String }
    }.ToImmutableDictionary();
}