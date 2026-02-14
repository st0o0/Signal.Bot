using Signal.Bot.Polling;

namespace Signal.Bot;

/// <summary>
/// Represents a connection-related event during WebSocket lifecycle that is reported as an error for handling purposes.
/// This is not an actual error condition, but rather a notification mechanism for connection state changes
/// such as initial connection or reconnection events.
/// </summary>
/// <param name="Event">The specific connection event that occurred (see <see cref="ConnectionEvent"/> for possible values).</param>
public record ConnectionError(ConnectionEvent Event) : Error(null, ErrorSource.ConnectionHappened);