namespace Signal.Bot;

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