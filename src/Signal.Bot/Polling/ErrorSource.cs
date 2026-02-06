namespace Signal.Bot.Polling;

public enum ErrorSource
{
    Undefined = 0,
    MessageReceived,
    MessageReceivedTermination,
    DisconnectionHappened,
    DisconnectionHappenedTermination,

    ConnectionHappened,
    ConnectionHappenedTermination,
    FatalError
}