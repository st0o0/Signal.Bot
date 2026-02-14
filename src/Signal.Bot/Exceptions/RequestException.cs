using System.Net;

namespace Signal.Bot.Exceptions;

/// <summary>
/// Represents errors that occur during Signal Bot API requests, optionally including HTTP status code information.
/// </summary>
public class RequestException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with the failed request, if available.
    /// </summary>
    /// <value>
    /// The <see cref="System.Net.HttpStatusCode"/> of the failed request, or <see langword="null"/> if not applicable.
    /// </value>
    public HttpStatusCode? HttpStatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RequestException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message
    /// and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="httpStatusCode">The HTTP status code of the failed request.</param>
    public RequestException(string message, HttpStatusCode httpStatusCode) : base(message)
        => HttpStatusCode = httpStatusCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message,
    /// HTTP status code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="httpStatusCode">The HTTP status code of the failed request.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
    public RequestException(string message, HttpStatusCode httpStatusCode, Exception? innerException)
        : base(message, innerException) => HttpStatusCode = httpStatusCode;
}