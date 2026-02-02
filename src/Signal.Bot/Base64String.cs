namespace Signal.Bot;

public record Base64String
{
    private readonly string _value;

    public Base64String(string base64Value)
    {
        if (!string.IsNullOrEmpty(base64Value) && !IsValidBase64(base64Value))
        {
            throw new ArgumentException("Invalid Base64 string", nameof(base64Value));
        }

        _value = base64Value;
    }

    public static Base64String FromBytes(byte[] bytes) => new(Convert.ToBase64String(bytes));

    public byte[] ToBytes() => Convert.FromBase64String(_value);

    public override string ToString() => _value;

    public static implicit operator string(Base64String base64) => base64._value;
    public static implicit operator Base64String(string value) => new(value);

    private static bool IsValidBase64(string value)
        => Convert.TryFromBase64String(value, new byte[value.Length], out _);
}