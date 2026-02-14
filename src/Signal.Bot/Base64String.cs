namespace Signal.Bot;

/// <summary>
/// Represents a validated base64-encoded string with conversion utilities for byte arrays.
/// </summary>
public record Base64String
{
    private readonly string _value;

    private Base64String(string base64Value)
    {
        if (!string.IsNullOrEmpty(base64Value) && !IsValidBase64(base64Value))
        {
            throw new ArgumentException("Invalid Base64 string", nameof(base64Value));
        }

        _value = base64Value;
    }

    /// <summary>
    /// Creates a Base64String from a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to encode as base64.</param>
    /// <returns>A new Base64String instance.</returns>
    public static Base64String FromBytes(byte[] bytes) => new(Convert.ToBase64String(bytes));

    /// <summary>
    /// Converts the base64 string to its byte array representation.
    /// </summary>
    /// <returns>The decoded byte array.</returns>
    public byte[] ToBytes() => Convert.FromBase64String(_value);

    /// <summary>
    /// Returns the base64-encoded string value.
    /// </summary>
    /// <returns>The base64 string.</returns>
    public override string ToString() => _value;

    /// <summary>
    /// Implicitly converts a Base64String to a regular string.
    /// </summary>
    /// <param name="base64">The Base64String to convert.</param>
    public static implicit operator string(Base64String base64) => base64._value;

    /// <summary>
    /// Implicitly converts a string to a Base64String with validation.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <exception cref="ArgumentException">Thrown when the provided string is not valid base64.</exception>
    public static implicit operator Base64String(string value) => new(value);

    /// <summary>
    /// Validates whether a string is valid base64-encoded data.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is valid base64; otherwise, false.</returns>
    private static bool IsValidBase64(string value)
        => Convert.TryFromBase64String(value, new byte[value.Length], out _);
}