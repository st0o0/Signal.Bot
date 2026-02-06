namespace Signal.Bot.Polling;

public enum ErrorType
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