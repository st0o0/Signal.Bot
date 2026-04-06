using System.Text.Json.Serialization;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Serialization;

/// <summary>
/// Provides source-generated JSON serialization context for the Signal Bot API with optimized performance and AOT compatibility.
/// </summary>
/// <remarks>
/// This context is configured with:
/// <list type="bullet">
/// <item><description>Snake_case property naming convention to match the Signal Bot API format</description></item>
/// <item><description>Custom timestamp converter for handling Unix epoch timestamps</description></item>
/// <item><description>String-based enum serialization for better API compatibility</description></item>
/// <item><description>Default value ignoring to reduce payload size</description></item>
/// <item><description>All Signal Bot API types, requests, and collections pre-registered for source generation</description></item>
/// </list>
/// </remarks>
[JsonSourceGenerationOptions(
    Converters = [typeof(TimestampConverter), typeof(TimeSpanConverter)],
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    WriteIndented = false,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(About))]
[JsonSerializable(typeof(Acknowledged))]
[JsonSerializable(typeof(Attachment))]
[JsonSerializable(typeof(Configuration))]
[JsonSerializable(typeof(Contact))]
[JsonSerializable(typeof(DataMessage))]
[JsonSerializable(typeof(Device))]
[JsonSerializable(typeof(Envelope))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Group))]
[JsonSerializable(typeof(GroupInfo))]
[JsonSerializable(typeof(Identity))]
[JsonSerializable(typeof(LoggingConfiguration))]
[JsonSerializable(typeof(Mention))]
[JsonSerializable(typeof(Nickname))]
[JsonSerializable(typeof(Preview))]
[JsonSerializable(typeof(Profile))]
[JsonSerializable(typeof(ProfileCapabilities))]
[JsonSerializable(typeof(Quote))]
[JsonSerializable(typeof(RawDeviceLink))]
[JsonSerializable(typeof(Reaction))]
[JsonSerializable(typeof(ReadMessage))]
[JsonSerializable(typeof(ReceiptMessage))]
[JsonSerializable(typeof(ReceivedMessage))]
[JsonSerializable(typeof(Search))]
[JsonSerializable(typeof(SetUsername))]
[JsonSerializable(typeof(StickerPack))]
[JsonSerializable(typeof(SyncMessage))]
[JsonSerializable(typeof(TypingMessage))]
[JsonSerializable(typeof(CallMessage))]
[JsonSerializable(typeof(OfferMessage))]
[JsonSerializable(typeof(HangupMessage))]
[JsonSerializable(typeof(IceUpdateMessage))]
[JsonSerializable(typeof(PollResponse))]
// Requests
[JsonSerializable(typeof(AddDeviceRequest))]
[JsonSerializable(typeof(AddGroupAdminRequest))]
[JsonSerializable(typeof(AddGroupMemberRequest))]
[JsonSerializable(typeof(AddReactionRequest))]
[JsonSerializable(typeof(AddStickerPackRequest))]
[JsonSerializable(typeof(AddTypingIndicatorRequest))]
[JsonSerializable(typeof(BlockGroupRequest))]
[JsonSerializable(typeof(CreateGroupRequest))]
[JsonSerializable(typeof(GetAboutRequest))]
[JsonSerializable(typeof(GetAccountsRequest))]
[JsonSerializable(typeof(GetAttachmentRequest))]
[JsonSerializable(typeof(GetAttachmentsRequest))]
[JsonSerializable(typeof(GetConfigurationRequest))]
[JsonSerializable(typeof(GetContactRequest))]
[JsonSerializable(typeof(GetContactsRequest))]
[JsonSerializable(typeof(GetDevicesRequest))]
[JsonSerializable(typeof(GetGroupRequest))]
[JsonSerializable(typeof(GetGroupsRequest))]
[JsonSerializable(typeof(GetIdentitiesRequest))]
[JsonSerializable(typeof(GetQrCodeLinkRequest))]
[JsonSerializable(typeof(GetRawDeviceLinkRequest))]
[JsonSerializable(typeof(GetReceivedMessagesRequest))]
[JsonSerializable(typeof(GetStickerPacksRequest))]
[JsonSerializable(typeof(JoinGroupRequest))]
[JsonSerializable(typeof(QuitGroupRequest))]
[JsonSerializable(typeof(RateLimitChallengeRequest))]
[JsonSerializable(typeof(RegisterNumberRequest))]
[JsonSerializable(typeof(RemoteDeleteRequest))]
[JsonSerializable(typeof(RemoveAttachmentRequest))]
[JsonSerializable(typeof(RemoveGroupAdminRequest))]
[JsonSerializable(typeof(RemoveGroupMemberRequest))]
[JsonSerializable(typeof(RemoveGroupRequest))]
[JsonSerializable(typeof(RemovePinRequest))]
[JsonSerializable(typeof(RemoveReactionRequest))]
[JsonSerializable(typeof(RemoveTypingIndicatorRequest))]
[JsonSerializable(typeof(RemoveUsernameRequest))]
[JsonSerializable(typeof(SearchNumbersRequest))]
[JsonSerializable(typeof(SendMessageRequest))]
[JsonSerializable(typeof(SendReceiptsRequest))]
[JsonSerializable(typeof(SetConfigurationRequest))]
[JsonSerializable(typeof(SetPinRequest))]
[JsonSerializable(typeof(SetUsernameRequest))]
[JsonSerializable(typeof(SyncContactsRequest))]
[JsonSerializable(typeof(TrustIdentityRequest))]
[JsonSerializable(typeof(UnregisterDeviceRequest))]
[JsonSerializable(typeof(UpdateAccountSettingsRequest))]
[JsonSerializable(typeof(UpdateContactRequest))]
[JsonSerializable(typeof(UpdateGroupRequest))]
[JsonSerializable(typeof(UpdateProfileRequest))]
[JsonSerializable(typeof(VerifyNumberRequest))]
[JsonSerializable(typeof(AddPollRequest))]
[JsonSerializable(typeof(ClosePollRequest))]
[JsonSerializable(typeof(VotePollRequest))]

// Arrays/Collections
[JsonSerializable(typeof(List<Group>))]
[JsonSerializable(typeof(List<Attachment>))]
[JsonSerializable(typeof(List<ReadMessage>))]
[JsonSerializable(typeof(List<Preview>))]
[JsonSerializable(typeof(List<Mention>))]
[JsonSerializable(typeof(List<Device>))]
[JsonSerializable(typeof(List<Identity>))]
[JsonSerializable(typeof(List<Contact>))]
[JsonSerializable(typeof(List<Search>))]
[JsonSerializable(typeof(List<StickerPack>))]
[JsonSerializable(typeof(List<string>))]
public partial class JsonBotSerializerContext : JsonSerializerContext;