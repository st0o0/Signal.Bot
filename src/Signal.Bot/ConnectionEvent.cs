namespace Signal.Bot;

/// <summary>
/// Defines the types of connection events that can occur during WebSocket communication.
/// </summary>
public enum ConnectionEvent
{
    /// <summary>
    /// Undefined or unknown connection event.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Initial connection has been established.
    /// </summary>
    Initialized = 1,

    /// <summary>
    /// Connection has been re-established after a disconnect.
    /// </summary>
    Reconnected = 2
}