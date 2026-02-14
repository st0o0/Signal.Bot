using Signal.Bot.Polling;

namespace Signal.Bot;

/// <summary>
/// Represents an error or event that occurs during Signal Bot operations, with an optional exception and categorized error type.
/// This serves as the base record for specific error types like <see cref="ConnectionError"/> and <see cref="DisconnectionError"/>.
/// </summary>
/// <param name="Exception">The exception that caused the error, or null if this represents an event rather than an actual error condition.</param>
/// <param name="Source">The type/category of the error for classification and handling purposes (see <see cref="ErrorSource"/>).</param>
public record Error(Exception? Exception, ErrorSource Source);