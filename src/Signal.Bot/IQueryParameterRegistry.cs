namespace Signal.Bot;

/// <summary>
/// Defines a registry for managing query parameters in HTTP requests, supporting single values and collections.
/// </summary>
public interface IQueryParameterRegistry
{
    /// <summary>
    /// Adds a query parameter and returns the value for method chaining.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value. Must be a non-nullable type.</typeparam>
    /// <param name="valueName">The name of the query parameter (e.g., "limit", "offset").</param>
    /// <param name="value">The value to add for this parameter.</param>
    /// <param name="valueFactory">Optional factory function to convert the value to a string representation. If null, uses ToString().</param>
    /// <returns>The original value that was added, enabling fluent syntax.</returns>
    T AddAndGet<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull;

    /// <summary>
    /// Adds a single query parameter to the registry.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value. Must be a non-nullable type.</typeparam>
    /// <param name="valueName">The name of the query parameter (e.g., "limit", "offset").</param>
    /// <param name="value">The value to add for this parameter.</param>
    /// <param name="valueFactory">Optional factory function to convert the value to a string representation. If null, uses ToString().</param>
    void Add<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull;

    /// <summary>
    /// Adds multiple values for a single query parameter, typically for array-style parameters.
    /// </summary>
    /// <typeparam name="T">The type of the parameter values. Must be a non-nullable type.</typeparam>
    /// <param name="valueName">The name of the query parameter (e.g., "numbers", "recipients").</param>
    /// <param name="values">The collection of values to add for this parameter.</param>
    /// <param name="valueFactory">Optional factory function to convert each value to a string representation. If null, uses ToString().</param>
    /// <remarks>
    /// Multiple values will typically be serialized as repeated query parameters (e.g., ?numbers=1&amp;numbers=2&amp;numbers=3).
    /// </remarks>
    void AddRange<T>(string valueName, IEnumerable<T> values, Func<T, string>? valueFactory = null) where T : notnull;
}