namespace Signal.Bot.Polling;

/// <summary>The source of the error</summary>
public enum ErrorSource
{
    Undefined = 0,
    MessageReceived,
    MessageReceivedTermination,
    DisconnectionHappened,
    DisconnectionHappenedTermination,

    ReconnectionHappened,
    ReconnectionHappenedTermination,
    FatalError
}