using System.Net.WebSockets;
using Signal.Bot.Polling;

namespace Signal.Bot;

/// <summary>
/// Represents a disconnection event that occurred during WebSocket communication.
/// Contains information about the disconnection type, close status, reason, and provides methods 
/// to control automatic reconnection and graceful closing behavior.
/// </summary>
/// <param name="Event">The type of disconnection event that occurred (see <see cref="DisconnectionEvent"/>).</param>
/// <param name="CloseStatus">The WebSocket close status code, if available.</param>
/// <param name="CloseStatusDescription">Optional human-readable description of why the connection was closed.</param>
/// <param name="SubProtocol">The WebSocket sub-protocol that was in use, if any.</param>
/// <param name="Exception">The exception that caused the disconnection, if applicable.</param>
public record DisconnectionError(
    DisconnectionEvent Event,
    WebSocketCloseStatus? CloseStatus,
    string? CloseStatusDescription = null,
    string? SubProtocol = null,
    Exception? Exception = null)
    : Error(Exception, ErrorSource.DisconnectionHappened)
{
    internal Action? CancelReconnectionAction;

    internal Action? CancelClosingAction;

    /// <summary>
    /// Cancels any automatic reconnection attempt that may be in progress or scheduled.
    /// Call this method to prevent the client from automatically reconnecting after a disconnection.
    /// </summary>
    public void CancelReconnection() => CancelReconnectionAction?.Invoke();

    /// <summary>
    /// Cancels the graceful closing process of the connection.
    /// Call this method to abort a connection close that is in progress.
    /// </summary>
    public void CancelClosing() => CancelClosingAction?.Invoke();
}