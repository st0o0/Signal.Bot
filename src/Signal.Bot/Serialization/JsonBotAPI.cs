using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Serialization;

public static class JsonBotAPI
{
    public static JsonSerializerOptions Options { get; }

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

    public static JsonTypeInfo Get(Type key)
    {
        return TypeInfos.TryGetValue(key, out var typeInfo)
            ? typeInfo
            : throw new InvalidOperationException($"No JsonTypeInfo for {key.Name}");
    }

    public static JsonTypeInfo<T> Get<T>() => (Get(typeof(T)) as JsonTypeInfo<T>)!;

    private static readonly ImmutableDictionary<Type, JsonTypeInfo> TypeInfos = new Dictionary<Type, JsonTypeInfo>
    {
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
        { typeof(GroupV2Info), JsonBotSerializerContext.Default.GroupV2Info },
        { typeof(Identity), JsonBotSerializerContext.Default.Identity },
        { typeof(LoggingConfiguration), JsonBotSerializerContext.Default.LoggingConfiguration },
        { typeof(Mention), JsonBotSerializerContext.Default.Mention },
        { typeof(Nickname), JsonBotSerializerContext.Default.Nickname },
        { typeof(PreviewData), JsonBotSerializerContext.Default.PreviewData },
        { typeof(Profile), JsonBotSerializerContext.Default.Profile },
        { typeof(ProfileCapabilities), JsonBotSerializerContext.Default.ProfileCapabilities },
        { typeof(QuoteData), JsonBotSerializerContext.Default.QuoteData },
        { typeof(RawDeviceLink), JsonBotSerializerContext.Default.RawDeviceLink },
        { typeof(ReactionData), JsonBotSerializerContext.Default.ReactionData },
        { typeof(ReadMessage), JsonBotSerializerContext.Default.ReadMessage },
        { typeof(ReceiptMessage), JsonBotSerializerContext.Default.ReceiptMessage },
        { typeof(ReceivedMessage), JsonBotSerializerContext.Default.ReceivedMessage },
        { typeof(Search), JsonBotSerializerContext.Default.Search },
        { typeof(SetUsername), JsonBotSerializerContext.Default.SetUsername },
        { typeof(StickerPack), JsonBotSerializerContext.Default.StickerPack },
        { typeof(SyncMessage), JsonBotSerializerContext.Default.SyncMessage },
        { typeof(TypingMessage), JsonBotSerializerContext.Default.TypingMessage },
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
        // Arrays
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
        { typeof(string), JsonBotSerializerContext.Default.String },
    }.ToImmutableDictionary();
}