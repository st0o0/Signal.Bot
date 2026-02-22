using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class MessageSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestReceivedMessage_OptionalFieldsMissing_DeserializesCorrectly()
    {
        // Arrange
        const string json = "{\"account\": \"msg123\", \"envelope\": {\"source\": \"src123\"}}";

        // Act
        var result = JsonSerializer.Deserialize<ReceivedMessage>(json, JsonBotAPI.Options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("msg123", result.Account);
        Assert.NotNull(result.Envelope);
        Assert.Equal("src123", result.Envelope.Source);
        Assert.Null(result.Envelope.DataMessage);
        Assert.Null(result.Envelope.ReceiptMessage);
        Assert.Null(result.Envelope.TypingMessage);
    }

    [Fact(Timeout = 5000)]
    public void TestReceivedMessageSerializationAndDeserialization()
    {
        // Arrange
        var receivedMessage = new ReceivedMessage
        {
            Account = "msg123",
            Envelope = new Envelope
            {
                SourceId = Guid.Empty,
                SourceNumber = "msg123",
                Source = "msg123",
                DataMessage = new DataMessage
                {
                    Message = "Hello, World!"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(receivedMessage);
        var deserializedReceivedMessage = JsonSerializer.Deserialize<ReceivedMessage>(json);

        // Assert
        Assert.NotNull(deserializedReceivedMessage);
        Assert.NotNull(deserializedReceivedMessage.Envelope);
        Assert.NotNull(deserializedReceivedMessage.Envelope.DataMessage);
        Assert.Equal(receivedMessage.Account, deserializedReceivedMessage.Account);
        Assert.Equal(receivedMessage.Envelope.DataMessage.Message,
            deserializedReceivedMessage.Envelope.DataMessage.Message);
    }

    [Fact(Timeout = 5000)]
    public void TestRemoteDeleteMessageSerializationAndDeserialization()
    {
        // Arrange
        var remoteDeleteMessage = new Acknowledged
        {
            Timestamp = DateTime.Now
        };

        // Act
        var json = JsonSerializer.Serialize(remoteDeleteMessage);
        var deserializedRemoteDeleteMessage = JsonSerializer.Deserialize<Acknowledged>(json);

        // Assert
        Assert.NotNull(deserializedRemoteDeleteMessage);
        Assert.Equal(remoteDeleteMessage.Timestamp, deserializedRemoteDeleteMessage.Timestamp);
    }

    [Fact(Timeout = 5000)]
    public void TestSendMessageRequestSerializationAndDeserialization()
    {
        // Arrange
        var sendMessageRequest = new SendMessageRequest
        {
            Message = "recipientUuid123",
            QuoteAuthor = "Hello, World!"
        };

        // Act
        var json = JsonSerializer.Serialize(sendMessageRequest);
        var deserializedSendMessageRequest = JsonSerializer.Deserialize<SendMessageRequest>(json);

        // Assert
        Assert.NotNull(deserializedSendMessageRequest);
        Assert.Equal(sendMessageRequest.Message, deserializedSendMessageRequest.Message);
        Assert.Equal(sendMessageRequest.QuoteAuthor, deserializedSendMessageRequest.QuoteAuthor);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_SignalMessage_WithDataMessage_MapsAllFields()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Alice Bob",
                                "sourceNumber": "+4915112345678",
                                "sourceUuid": "f7e7c46b-8f52-4d87-a977-f352ad7f5667",
                                "sourceName": "Alice",
                                "sourceDevice": 1,
                                "timestamp": 1000,
                                "serverReceivedTimestamp": 2000,
                                "serverDeliveredTimestamp": 3000,
                                "dataMessage": {
                                  "timestamp": 4000,
                                  "message": "Hello World",
                                  "expiresInSeconds": 604800,
                                  "isExpirationUpdate": false,
                                  "viewOnce": false
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.Equal("MyAccount", result.Account);
        Assert.NotNull(result.Envelope);
        var env = result.Envelope;
        Assert.Equal("Alice Bob", env.Source);
        Assert.Equal("+4915112345678", env.SourceNumber);
        Assert.Equal(Guid.Parse("f7e7c46b-8f52-4d87-a977-f352ad7f5667"), env.SourceId);
        Assert.Equal("Alice", env.SourceName);
        Assert.Equal(1, env.SourceDevice);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(1000), env.Timestamp);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(2000), env.ServerReceived);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(3000), env.ServerDelivered);
        Assert.Null(env.TypingMessage);
        Assert.Null(env.SyncMessage);

        var dm = env.DataMessage;
        Assert.NotNull(dm);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(4000), dm.Timestamp);
        Assert.Equal("Hello World", dm.Message);
        Assert.Equal(TimeSpan.FromSeconds(604800), dm.ExpiresIn);
        Assert.False(dm.IsExpirationUpdate);
        Assert.False(dm.ViewOnce);
        Assert.Null(dm.Reaction);
        Assert.Null(dm.Attachments);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_DataMessage_NullMessage_IsAllowed()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "", "sourceNumber": "", "sourceUuid": "00000000-0000-0000-0000-000000000000",
                                "sourceName": "", "sourceDevice": 0,
                                "timestamp": 0, "serverReceivedTimestamp": 0, "serverDeliveredTimestamp": 0,
                                "dataMessage": {
                                  "timestamp": 0,
                                  "message": null,
                                  "expiresInSeconds": 600,
                                  "isExpirationUpdate": true,
                                  "viewOnce": false
                                }
                              },
                              "account": ""
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.DataMessage);
        Assert.Null(result.Envelope.DataMessage.Message);
        Assert.True(result.Envelope.DataMessage.IsExpirationUpdate);
        Assert.Equal(TimeSpan.FromSeconds(600), result.Envelope.DataMessage.ExpiresIn);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithTypingMessage_MapsAllFields()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Bob", "sourceNumber": "+49", "sourceUuid": "aa22d86e-84e8-4570-8b0a-128e0c6ab6b8",
                                "sourceName": "Bob", "sourceDevice": 2,
                                "timestamp": 5000, "serverReceivedTimestamp": 6000, "serverDeliveredTimestamp": 7000,
                                "typingMessage": {
                                  "action": "STARTED",
                                  "timestamp": 8000
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;
        Assert.NotNull(result.Envelope);
        var typing = result.Envelope.TypingMessage;

        Assert.NotNull(typing);
        Assert.Equal(TypingAction.Started, typing.Action);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(8000), typing.Timestamp);
        Assert.Null(result.Envelope.DataMessage);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_ReadMessages_MapsAllFields()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Me", "sourceNumber": "+49", "sourceUuid": "8ec75ef6-0774-4deb-b623-627ec134b072",
                                "sourceName": "Me", "sourceDevice": 3,
                                "timestamp": 100, "serverReceivedTimestamp": 200, "serverDeliveredTimestamp": 300,
                                "syncMessage": {
                                  "readMessages": [
                                    {
                                      "sender": "Alice",
                                      "senderNumber": "+49123",
                                      "senderUuid": "17d4540e-6337-4271-a77e-3e6ed0907dfb",
                                      "timestamp": 9000
                                    },
                                    {
                                      "sender": "Bob",
                                      "senderNumber": "+49456",
                                      "senderUuid": "068b6bfd-9317-49f0-b00e-d399a66fee45",
                                      "timestamp": 10000
                                    }
                                  ]
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;
        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.ReadMessages);
        var reads = result.Envelope.SyncMessage.ReadMessages;

        Assert.Equal(2, reads.Count);

        Assert.Equal("Alice", reads[0].Sender);
        Assert.Equal("+49123", reads[0].SenderNumber);
        Assert.Equal(Guid.Parse("17d4540e-6337-4271-a77e-3e6ed0907dfb"), reads[0].SenderId);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(9000), reads[0].Timestamp);

        Assert.Equal("Bob", reads[1].Sender);
        Assert.Equal(Guid.Parse("068b6bfd-9317-49f0-b00e-d399a66fee45"), reads[1].SenderId);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_SentMessage_ExpirationUpdate()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Me", "sourceNumber": "+49", "sourceUuid": "5b948386-56d9-4058-9b19-aac8e12a836d",
                                "sourceName": "Me", "sourceDevice": 1,
                                "timestamp": 100, "serverReceivedTimestamp": 200, "serverDeliveredTimestamp": 300,
                                "syncMessage": {
                                  "sentMessage": {
                                    "destination": "Carol",
                                    "destinationNumber": "+49789",
                                    "destinationUuid": "399e537d-6ff9-4ee4-9561-0f5592659fda",
                                    "timestamp": 11000,
                                    "message": null,
                                    "expiresInSeconds": 2592000,
                                    "isExpirationUpdate": true,
                                    "viewOnce": false
                                  }
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;
        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage);
        var sent = result.Envelope.SyncMessage.SentMessage;

        Assert.Equal("Carol", sent.Destination);
        Assert.Equal("+49789", sent.DestinationNumber);
        Assert.Equal(Guid.Parse("399e537d-6ff9-4ee4-9561-0f5592659fda"), sent.DestinationId);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(11000), sent.Timestamp);
        Assert.Null(sent.Message);
        Assert.Equal(TimeSpan.FromSeconds(2592000), sent.ExpiresIn);
        Assert.True(sent.IsExpirationUpdate);
        Assert.False(sent.ViewOnce);
        Assert.Null(sent.Reaction);
        Assert.Null(sent.Attachments);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_SentMessage_WithReaction()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Me", "sourceNumber": "+49", "sourceUuid": "2def8440-2353-4c83-a9f3-9d066ac426ac",
                                "sourceName": "Me", "sourceDevice": 1,
                                "timestamp": 100, "serverReceivedTimestamp": 200, "serverDeliveredTimestamp": 300,
                                "syncMessage": {
                                  "sentMessage": {
                                    "destination": "Dave",
                                    "destinationNumber": "+49000",
                                    "destinationUuid": "5aca3f9f-4121-4f1e-9660-011c0473ec58",
                                    "timestamp": 12000,
                                    "message": null,
                                    "expiresInSeconds": 86400,
                                    "isExpirationUpdate": false,
                                    "viewOnce": false,
                                    "reaction": {
                                      "emoji": "👍",
                                      "targetAuthor": "Eve",
                                      "targetAuthorNumber": "+49111",
                                      "targetAuthorUuid": "2450bb5a-d476-44b9-8986-4866a48f1c65",
                                      "targetSentTimestamp": 13000,
                                      "isRemove": true
                                    }
                                  }
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;
        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage);
        var reaction = result.Envelope.SyncMessage.SentMessage.Reaction!;

        Assert.Equal("👍", reaction.Emoji);
        Assert.Equal("Eve", reaction.TargetAuthor);
        Assert.Equal(Guid.Parse("2450bb5a-d476-44b9-8986-4866a48f1c65"), reaction.TargetAuthorUuid);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(13000), reaction.TargetSent);
        Assert.True(reaction.IsRemove);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_DataMessage_WithReaction()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Frank", "sourceNumber": "+49222", "sourceUuid": "8fbebe08-77f0-49d7-a979-324f275a3d00",
                                "sourceName": "Frank", "sourceDevice": 5,
                                "timestamp": 100, "serverReceivedTimestamp": 200, "serverDeliveredTimestamp": 300,
                                "dataMessage": {
                                  "timestamp": 14000,
                                  "message": null,
                                  "expiresInSeconds": 600,
                                  "isExpirationUpdate": true,
                                  "viewOnce": false,
                                  "reaction": {
                                    "emoji": "❤️",
                                    "targetAuthor": "Grace",
                                    "targetAuthorNumber": "+49333",
                                    "targetAuthorUuid": "f1badf5e-36f7-4d67-a2cc-6caf86296693",
                                    "targetSentTimestamp": 15000,
                                    "isRemove": false
                                  }
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;
        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.DataMessage);
        Assert.NotNull(result.Envelope.DataMessage.Reaction);
        var reaction = result.Envelope.DataMessage.Reaction;

        Assert.Equal("❤️", reaction.Emoji);
        Assert.Equal("Grace", reaction.TargetAuthor);
        Assert.Equal(Guid.Parse("f1badf5e-36f7-4d67-a2cc-6caf86296693"), reaction.TargetAuthorUuid);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(15000), reaction.TargetSent);
        Assert.False(reaction.IsRemove);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_SentMessage_WithAttachment()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Me", "sourceNumber": "+49", "sourceUuid": "3dcd7227-7974-4f67-bd3b-73c28df7c53b",
                                "sourceName": "Me", "sourceDevice": 1,
                                "timestamp": 100, "serverReceivedTimestamp": 200, "serverDeliveredTimestamp": 300,
                                "syncMessage": {
                                  "sentMessage": {
                                    "destination": "Henry",
                                    "destinationNumber": "+49444",
                                    "destinationUuid": "d74feb8d-78e3-4619-b367-cda59d8c9d49",
                                    "timestamp": 16000,
                                    "message": null,
                                    "expiresInSeconds": 1800,
                                    "isExpirationUpdate": false,
                                    "viewOnce": false,
                                    "attachments": [
                                      {
                                        "contentType": "audio/aac",
                                        "filename": null,
                                        "id": "attach-001",
                                        "size": 62869,
                                        "width": null,
                                        "height": null,
                                        "caption": null,
                                        "uploadTimestamp": 17000
                                      }
                                    ]
                                  }
                                }
                              },
                              "account": "MyAccount"
                            }
                            """;

        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage.Attachments);
        var attachments = result.Envelope.SyncMessage.SentMessage.Attachments;

        Assert.Single(attachments);
        var a = attachments[0];
        Assert.Equal("audio/aac", a.ContentType);
        Assert.Null(a.Filename);
        Assert.Equal("attach-001", a.Id);
        Assert.Equal(62869L, a.Size);
        Assert.Null(a.Width);
        Assert.Null(a.Height);
        Assert.Null(a.Caption);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(17000), a.UploadTimestamp);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_SentMessage_WithGroupInfo()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Redacted Leberkas",
                                "sourceNumber": "Leberkas Ketchup",
                                "sourceUuid": "f749c279-fa3a-4394-a19c-dda3f8d6eb6b",
                                "sourceName": "Lorem Anonym",
                                "sourceDevice": 45418,
                                "timestamp": 1773459844162,
                                "serverReceivedTimestamp": 1771188481910,
                                "serverDeliveredTimestamp": 1771739602766,
                                "syncMessage": {
                                  "sentMessage": {
                                    "destination": null,
                                    "destinationNumber": null,
                                    "destinationUuid": null,
                                    "timestamp": 1771043387272,
                                    "message": "Amet Dolor",
                                    "expiresInSeconds": 900,
                                    "isExpirationUpdate": true,
                                    "viewOnce": true,
                                    "groupInfo": {
                                      "groupId": "Dolor Servus",
                                      "groupName": "Anonym Lorem",
                                      "revision": 6455,
                                      "type": "DELIVER"
                                    }
                                  }
                                }
                              },
                              "account": "Amet Dolor"
                            }
                            """;
        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage.GroupInfo);
        var groupInfo = result.Envelope.SyncMessage.SentMessage.GroupInfo;

        Assert.Equal("Dolor Servus", groupInfo.Id);
        Assert.Equal("Anonym Lorem", groupInfo.Name);
        Assert.Equal(6455, groupInfo.Revision);
        Assert.Equal("DELIVER", groupInfo.Type);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithSyncMessage_SentMessage_WithPreviews()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "KEKW Lorem",
                                "sourceNumber": "Dolor Dolor",
                                "sourceUuid": "dc8a8372-019f-451d-9ad9-705a43a0050d",
                                "sourceName": "Ketchup Dolor",
                                "sourceDevice": 92706,
                                "timestamp": 1770236077599,
                                "serverReceivedTimestamp": 1771803556110,
                                "serverDeliveredTimestamp": 1771040747066,
                                "syncMessage": {
                                  "sentMessage": {
                                    "destination": null,
                                    "destinationNumber": null,
                                    "destinationUuid": null,
                                    "timestamp": 1773216132161,
                                    "message": "Amet Lorem",
                                    "expiresInSeconds": 7200,
                                    "isExpirationUpdate": false,
                                    "viewOnce": true,
                                    "previews": [
                                      {
                                        "url": "Dolor Amet",
                                        "title": "Lorem Lorem",
                                        "description": "Leberkas KEKW",
                                        "image": {
                                          "contentType": "Amet Anonym",
                                          "filename": null,
                                          "id": "Ipsum Lorem",
                                          "size": 7828,
                                          "width": 98039,
                                          "height": 53244,
                                          "caption": null,
                                          "uploadTimestamp": 1771892460342
                                        }
                                      }
                                    ],
                                    "groupInfo": {
                                      "groupId": "Ketchup Ketchup",
                                      "groupName": "Ketchup Lorem",
                                      "revision": 22044,
                                      "type": "DELIVER"
                                    }
                                  }
                                }
                              },
                              "account": "Ketchup KEKW"
                            }
                            """;
        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.SyncMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage);
        Assert.NotNull(result.Envelope.SyncMessage.SentMessage.Previews);
        var previews = result.Envelope.SyncMessage.SentMessage.Previews;
        Assert.Single(previews);
        var preview = previews[0];
        Assert.Equal("Lorem Lorem", preview.Title);
        Assert.Equal("Dolor Amet", preview.Url);
        Assert.Equal("Leberkas KEKW", preview.Description);
        Assert.NotNull(preview.Image);
        Assert.Equal("Ipsum Lorem", preview.Image.Id);
    }

    [Fact(Timeout = 5000)]
    public void Deserialize_ReceivedMessage_WithCallMessage()
    {
        const string json = """
                            {
                              "envelope": {
                                "source": "Servus Anonym",
                                "sourceNumber": "Servus Anonym",
                                "sourceUuid": "2ff185b5-4365-419a-b3c6-3bdfbf629f8a",
                                "sourceName": "Lorem Leberkas",
                                "sourceDevice": 63282,
                                "timestamp": 1772582585300,
                                "serverReceivedTimestamp": 1773642716538,
                                "serverDeliveredTimestamp": 1769416216320,
                                "callMessage": {
                                  "hangupMessage": {
                                    "id": 65669,
                                    "type": "ACCEPTED",
                                    "deviceId": 66344
                                  },
                                  "offerMessage": {
                                    "id": 93200,
                                    "type": "AUDIO_CALL",
                                    "opaque": "KEKW Ketchup"
                                  },
                                  "iceUpdateMessages": [
                                  {
                                    "id": 94927,
                                    "opaque": "Ipsum Ketchup"
                                  },
                                  {
                                    "id": 54408,
                                    "opaque": "KEKW KEKW"
                                  },
                                  {
                                    "id": 40422,
                                    "opaque": "Lorem Servus"
                                  },
                                  {
                                    "id": 96895,
                                    "opaque": "Ketchup KEKW"
                                  },
                                  {
                                    "id": 31083,
                                    "opaque": "KEKW Leberkas"
                                  },
                                  {
                                    "id": 67296,
                                    "opaque": "Amet Lorem"
                                  }
                                ]
                                }
                              },
                              "account": "Amet Ipsum"
                            }
                            """;
        var result = JsonSerializer.Deserialize(json, JsonBotAPI.Get<ReceivedMessage>())!;

        Assert.NotNull(result.Envelope);
        Assert.NotNull(result.Envelope.CallMessage);
        var callMessage = result.Envelope.CallMessage;
        Assert.NotNull(callMessage.HangupMessage);
        Assert.Equal(65669, callMessage.HangupMessage.Id);
        Assert.Equal(66344, callMessage.HangupMessage.DeviceId);
        Assert.Equal(HangupType.Accepted, callMessage.HangupMessage.Type);
    }
}