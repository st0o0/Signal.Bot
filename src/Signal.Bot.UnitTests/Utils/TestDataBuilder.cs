namespace Signal.Bot.UnitTests.Utils;

public static class TestDataBuilder
{
    public static class PhoneNumbers
    {
        public const string Bot = "+491701234567";
        public const string Recipient1 = "+491709876543";
        public const string Recipient2 = "+491701111111";
        public const string Recipient3 = "+491702222222";
    }

    public static class GroupIds
    {
        public const string TestGroup1 = "group.ckRzaEd4VmRzNnJaASAEsasa";
        public const string TestGroup2 = "group.xyz789abc123def456";
    }

    public static object CreateMessageEnvelope(
        string from = null!,
        string message = "Test message",
        long? timestamp = null)
    {
        timestamp ??= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return new
        {
            envelope = new
            {
                source = from,
                sourceNumber = from,
                timestamp,
                dataMessage = new
                {
                    timestamp,
                    message,
                    expiresInSeconds = 0,
                    viewOnce = false
                }
            },
            account = PhoneNumbers.Bot
        };
    }

    public static object CreateGroupInfo(
        string groupId = null!,
        string name = "Test Group",
        string[] members = null!)
    {
        return new
        {
            id = groupId,
            name,
            members,
            description = "Test group description"
        };
    }

    public static object CreateErrorResponse(
        string message = "Error occurred",
        int? code = null)
    {
        var error = new Dictionary<string, object>
        {
            { "error", message }
        };

        if (code.HasValue)
        {
            error["code"] = code.Value;
        }

        return error;
    }

    public static string CreateBase64Image(int size = 100)
    {
        var imageData = new byte[size];
        new Random().NextBytes(imageData);
        return Convert.ToBase64String(imageData);
    }
}
