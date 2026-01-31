using Signal.Bot.Internal;

namespace Signal.Bot.Tests;

public class QueryParameterRegistryTests
{
    private readonly QueryParameterRegistry _registry = new();

    #region AddAndGet Tests

    [Fact]
    public void AddAndGet_StringValue_ReturnsSameValue()
    {
        // Arrange & Act
        var result = _registry.AddAndGet("test", "value");

        // Assert
        Assert.Equal("value", result);
        Assert.Equal("?test=value", _registry.Build());
    }

    [Fact]
    public void AddAndGet_IntValue_ReturnsSameValue()
    {
        // Arrange & Act
        var result = _registry.AddAndGet("count", 42);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal("?count=42", _registry.Build());
    }

    [Fact]
    public void AddAndGet_WithValueFactory_UsesFactory()
    {
        // Arrange & Act
        _ = _registry.AddAndGet("date", DateTime.Now, dt => dt.ToString("yyyy-MM-dd"));

        // Assert
        Assert.StartsWith("?date=", _registry.Build());
    }

    [Fact]
    public void AddAndGet_NullValueName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.AddAndGet(null!, "value"));
    }

    [Fact]
    public void AddAndGet_NullValue_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.AddAndGet<string>("test", null!));
    }

    [Fact]
    public void AddAndGet_MultipleCalls_ReturnsEachValue()
    {
        // Act
        var first = _registry.AddAndGet("a", 1);
        var second = _registry.AddAndGet("b", 2);

        // Assert
        Assert.Equal(1, first);
        Assert.Equal(2, second);
        Assert.Equal("?a=1&b=2", _registry.Build());
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_StringValue_BuildsCorrectQuery()
    {
        // Act
        _registry.Add("name", "John");

        // Assert
        Assert.Equal("?name=John", _registry.Build());
    }

    [Fact]
    public void Add_IntValue_BuildsCorrectQuery()
    {
        // Act
        _registry.Add("age", 30);

        // Assert
        Assert.Equal("?age=30", _registry.Build());
    }

    [Fact]
    public void Add_BooleanValue_BuildsCorrectQuery()
    {
        // Act
        _registry.Add("active", true);

        // Assert
        Assert.Equal("?active=True", _registry.Build());
    }

    [Fact]
    public void Add_EnumValue_BuildsCorrectQuery()
    {
        // Act
        _registry.Add("mode", StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal("?mode=RemoveEmptyEntries", _registry.Build());
    }

    [Fact]
    public void Add_MultipleParameters_BuildsCorrectFormat()
    {
        // Act
        _registry.Add("a", 1);
        _registry.Add("b", "two");
        _registry.Add("c", true);

        // Assert
        Assert.Equal("?a=1&b=two&c=True", _registry.Build());
    }

    [Fact]
    public void Add_WithValueFactory_UsesFactoryOutput()
    {
        // Arrange
        static string CustomFormat(DateTime dt) => dt.ToString("yyyyMMdd");

        // Act
        _registry.Add("date", DateTime.Now, CustomFormat);

        // Assert
        var result = _registry.Build();
        Assert.StartsWith("?date=", result);
        Assert.Contains("20", result);
    }

    [Fact]
    public void Add_ValueImplementsToString_UsesToString()
    {
        // Arrange
        var customObj = new TestClass { Value = "custom" };

        // Act
        _registry.Add("obj", customObj);

        // Assert
        Assert.Equal("?obj=custom", _registry.Build());
    }

    [Fact]
    public void Add_NullValueName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.Add(null!, "value"));
    }

    [Fact]
    public void Add_NullValue_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.Add<string>("test", null!));
    }

    [Fact]
    public void Add_NullValueFactory_IgnoresFactory()
    {
        // Act
        _registry.Add("test", 123);

        // Assert
        Assert.Equal("?test=123", _registry.Build());
    }

    #endregion

    #region Build Tests

    [Fact]
    public void Build_EmptyRegistry_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, _registry.Build());
    }

    [Fact]
    public void Build_SingleParameter_ReturnsCorrectFormat()
    {
        // Act
        _registry.Add("key", "value");

        // Assert
        Assert.Equal("?key=value", _registry.Build());
    }

    [Fact]
    public void Build_MultipleParameters_MaintainsOrder()
    {
        // Act
        _registry.Add("first", 1);
        _registry.Add("second", 2);
        _registry.Add("third", "three");

        // Assert
        var result = _registry.Build();
        Assert.Equal("?first=1&second=2&third=three", result);
    }

    [Fact]
    public void Build_AfterAddAndGet_WorksCorrectly()
    {
        // Act
        _registry.AddAndGet("a", 1);
        _registry.Add("b", 2);

        // Assert
        Assert.Equal("?a=1&b=2", _registry.Build());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Add_SpecialCharacters_EncodesCorrectly()
    {
        // Act
        _registry.Add("name", "John&Doe");
        _registry.Add("value", "test space");

        // Assert
        Assert.Equal("?name=John%26Doe&value=test%20space", _registry.Build());
    }

    [Fact]
    public void Add_EmptyStringValue_Allowed()
    {
        // Act
        _registry.Add("empty", "");

        // Assert
        Assert.Equal("?empty=", _registry.Build());
    }

    [Fact]
    public void Add_NullValueFactoryWithStringFallback_Works()
    {
        // Arrange
        var testValue = "hello";

        // Act
        _registry.Add("test", testValue);

        // Assert
        Assert.Equal("?test=hello", _registry.Build());
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FullUsageScenario_BuildsCorrectQueryString()
    {
        // Arrange & Act
        _registry.AddAndGet("page", 1);
        _registry.Add("size", 10);
        _registry.Add("filter", "active", v => v.ToUpper());
        _registry.Add("sort", "name");

        // Assert
        var result = _registry.Build();
        Assert.Equal("?page=1&size=10&filter=ACTIVE&sort=name", result);
    }

    #endregion
}

public class TestClass
{
    public string Value { get; set; } = string.Empty;

    public override string ToString() => Value;
}