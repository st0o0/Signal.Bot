namespace Signal.Bot;

/// <summary>
/// Defines the different types of disconnection events that can occur during WebSocket communication.
/// </summary>
public enum DisconnectionEvent
{
    /// <summary>
    /// An undefined or unknown disconnection event.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The disconnection was initiated by the client.
    /// </summary>
    ClientInitiated = 1,

    /// <summary>
    /// The disconnection was initiated by the server.
    /// </summary>
    ServerInitiated = 2,

    /// <summary>
    /// The connection timed out due to inactivity or lack of response.
    /// </summary>
    TimedOut = 3,

    /// <summary>
    /// The connection was unexpectedly dropped or lost.
    /// </summary>
    Dropped = 4,

    /// <summary>
    /// The connection has been properly closed.
    /// </summary>
    Closed = 5
}