using System.Net;
using System.Net.WebSockets;
using Signal.Bot.Exceptions;
using Signal.Bot.Polling;
using WebSocket.Rx;

namespace Signal.Bot.UnitTests.Exceptions;

public class ErrorHandlingTests
{
    [Theory(Timeout = 5000)]
    [InlineData(ConnectReason.Initial, ConnectionType.Initial)]
    [InlineData(ConnectReason.Reconnect, ConnectionType.Reconnect)]
    public void ConnectionError_To_MapsCorrectly(ConnectReason reason, ConnectionType expectedType)
    {
        // Arrange
        var connectedInfo = new Connected(reason);

        // Act
        var error = connectedInfo.To() as ConnectionError;

        // Assert
        Assert.NotNull(error);
        Assert.Equal(expectedType, error.Type);
    }

    [Theory(Timeout = 5000)]
    [InlineData(DisconnectReason.ConnectionLost, DisconnectionType.ConnectionLost)]
    [InlineData(DisconnectReason.Timeout, DisconnectionType.Timeout)]
    [InlineData(DisconnectReason.ClientInitiated, DisconnectionType.ClientInitiated)]
    [InlineData(DisconnectReason.ServerInitiated, DisconnectionType.ServerInitiated)]
    [InlineData(DisconnectReason.Shutdown, DisconnectionType.Shutdown)]
    public void DisconnectionError_To_MapsCorrectly(DisconnectReason reason, DisconnectionType expectedType)
    {
        // Arrange
        var exception = new WebSocketException("Test");
        var disconnectedInfo = new Disconnected(reason, WebSocketCloseStatus.NormalClosure, "desc", "sub", exception);

        // Act
        var error = disconnectedInfo.To() as DisconnectionError;

        // Assert
        Assert.NotNull(error);
        Assert.Equal(expectedType, error.DisconnectionType);
        Assert.Equal(exception, error.Exception);
    }

    [Fact(Timeout = 5000)]
    public void DisconnectionError_CancelActions_InvokeCorrectly()
    {
        // Arrange
        var disconnectedInfo = new Disconnected(DisconnectReason.Timeout, null, null, null, null);
        var error = disconnectedInfo.To() as DisconnectionError;
        Assert.NotNull(error);

        var reconnectionCancelled = false;
        var closingCancelled = false;

        error.CancelReconnectionAction = () => reconnectionCancelled = true;
        error.CancelClosingAction = () => closingCancelled = true;

        // Act
        error.CancelReconnection();
        error.CancelClosing();

        // Assert
        Assert.True(reconnectionCancelled);
        Assert.True(closingCancelled);
    }

    [Fact(Timeout = 5000)]
    public void ErrorRecord_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("msg");
        const ErrorType errorType = ErrorType.FatalError;

        // Act
        var error = new Error(exception, errorType);

        // Assert
        Assert.Equal(exception, error.Exception);
        Assert.Equal(errorType, error.ErrorType);
    }

    [Fact(Timeout = 5000)]
    public void RequestException_SetsPropertiesCorrectly()
    {
        // Arrange
        const string message = "Error";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        var inner = new Exception("inner");

        // Act
        var ex = new RequestException(message, statusCode, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(statusCode, ex.HttpStatusCode);
        Assert.Equal(inner, ex.InnerException);
    }

    [Fact(Timeout = 5000)]
    public void RequestException_SetsPropertiesCorrectly2()
    {
        // Arrange
        const string message = "Error";
        var inner = new Exception("inner");

        // Act
        var ex = new RequestException(message, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
    }

    [Fact(Timeout = 5000)]
    public void RequestException_SetsPropertiesCorrectly3()
    {
        // Arrange
        const string message = "Error";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        // Act
        var ex = new RequestException(message, statusCode);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(statusCode, ex.HttpStatusCode);
    }
    
    [Fact(Timeout = 5000)]
    public void RequestException_SetsPropertiesCorrectly4()
    {
        // Arrange
        const string message = "Error";

        // Act
        var ex = new RequestException(message);

        // Assert
        Assert.Equal(message, ex.Message);
    }
}