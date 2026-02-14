namespace Signal.Bot.Polling;

/// <summary>
/// Defines the types of errors that can occur during message polling and WebSocket communication.
/// <para>Possible values:</para>
/// <list type="bullet">
/// <item><see cref="Undefined"/> - Undefined or unknown error type.</item>
/// <item><see cref="MessageReceived"/> - Error occurred while processing a received message.</item>
/// <item><see cref="MessageReceiveTerminated"/> - Message receiving was terminated due to an error.</item>
/// <item><see cref="DisconnectionHappened"/> - A disconnection event occurred.</item>
/// <item><see cref="DisconnectionHappenTerminated"/> - Disconnection handling was terminated due to an error.</item>
/// <item><see cref="ConnectionHappened"/> - A connection event occurred.</item>
/// <item><see cref="ConnectionHappenTerminated"/> - Connection handling was terminated due to an error.</item>
/// <item><see cref="Failed"/> - A general failure occurred.</item>
/// </list>
/// </summary>
public enum ErrorSource
{
    /// <summary>
    /// Undefined or unknown error type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Error occurred while processing a received message from the Signal server.
    /// </summary>
    MessageReceived,

    /// <summary>
    /// Message receiving process was terminated due to an error or cancellation.
    /// </summary>
    MessageReceiveTerminated,

    /// <summary>
    /// A WebSocket disconnection event occurred during polling.
    /// </summary>
    DisconnectionHappened,

    /// <summary>
    /// Disconnection event handling was terminated due to an error.
    /// </summary>
    DisconnectionHappenTerminated,

    /// <summary>
    /// A WebSocket connection event occurred during polling.
    /// </summary>
    ConnectionHappened,

    /// <summary>
    /// Connection event handling was terminated due to an error.
    /// </summary>
    ConnectionHappenTerminated,

    /// <summary>
    /// A general failure occurred that doesn't fit other specific error types.
    /// </summary>
    Failed
}