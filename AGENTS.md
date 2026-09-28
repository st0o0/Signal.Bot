# AGENTS.md

## Project

Signal.Bot -- typed C# NuGet client for the
[signal-cli REST API](https://github.com/bbernhard/signal-cli-rest-api).
Wraps every signal-cli endpoint as a strongly-typed request record, with R3
Observable streams for real-time message reception via WebSocket. AOT-compatible,
source-generated System.Text.Json serialization.

## Solution structure

```
src/
  signal.slnx
  Signal.Bot/                    # Core library (NuGet package)
  Signal.Bot.UnitTests/          # Unit tests (NSubstitute)
  Signal.Bot.IntegrationTests/   # Integration tests (WireMock.Net)
  Signal.Bot.Example/            # ASP.NET Core example app (PublishAot)
  Signal.Bot.Example.Env/        # Aspire AppHost for example
docs/                            # VitePress documentation site
```

## Build & test

All commands run from repo root:

```powershell
dotnet build src/signal.slnx
dotnet test src/signal.slnx
```

Solution file: `src/signal.slnx` (.slnx format). Always use the solution file,
never individual csproj files.

## Architecture guardrails

- **AOT compatibility is non-negotiable.** `IsAotCompatible=true` and
  `EnableTrimAnalyzer=true` are set. Never use reflection-based serialization,
  `dynamic`, `Activator.CreateInstance`, or any type not registered in
  `JsonBotSerializerContext`.
- **Wire-format contracts.** Never remove or change `[JsonPropertyName]`
  attributes on existing properties. These are the API wire format.
- **Serializer context registration.** Every new serializable type (request or
  response) must be registered as `[JsonSerializable(typeof(T))]` in
  `JsonBotSerializerContext`. Collection types too (e.g. `List<T>`).
- **Central Package Management.** All package versions live in
  `Directory.Packages.props`. Do not add `Version=` attributes in csproj files.
- **Dependency-light library.** Do not add NuGet packages without good reason.
  This is a library consumed by others.
- **No real API calls in tests.** Unit tests mock HttpClient via NSubstitute.
  Integration tests use WireMock.Net. Never call the real Signal API.

## Request/Response pattern

Every API call is a `record` inheriting `RequestBase` (no response body) or
`RequestBase<TResponse>` (typed response). The base record takes the API
endpoint path and HTTP method (defaults to POST):

```csharp
// GET request with typed response
public record GetAccountsRequest() : RequestBase<List<string>?>("v1/accounts", HttpMethod.Get);

// POST request with typed response
public record SendMessageRequest() : RequestBase<Acknowledged>("v2/send")
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("recipients")]
    public string[]? Recipients { get; set; }
}
```

### Adding a new API endpoint

1. Create the request record in `Requests/` (inherit `RequestBase` or
   `RequestBase<TResponse>`)
2. Create response type in `Types/` if needed
3. Add `[JsonSerializable(typeof(...))]` to `JsonBotSerializerContext` for
   request, response, and any collection types
4. Add extension method on `ISignalBotClient` in `Signal.Bot` namespace
5. Add unit test (inherit `BotTestBase`)
6. Add integration test (inherit `IntegrationTestBase`)

## Namespaces

- `Signal.Bot` -- core interfaces, client, options, extension methods
- `Signal.Bot.Args` -- event arguments
- `Signal.Bot.Exceptions` -- custom exceptions
- `Signal.Bot.Internal` -- internal helpers (InternalsVisibleTo test projects)
- `Signal.Bot.Polling` -- WebSocket receiver, polling
- `Signal.Bot.Requests` -- API request records
- `Signal.Bot.Serialization` -- JSON converters, serializer context
- `Signal.Bot.Types` -- API response/DTO types

## C# conventions

- File-scoped namespaces everywhere.
- `record` for all DTOs (requests and types) with `{ get; set; }` properties.
- XML doc comments on all public APIs (`GenerateDocumentationFile=true`).
- `[JsonPropertyName("snake_case")]` on all serialized properties.
- Nullable reference types enabled globally.
- `sealed` classes for implementations.
- `ConfigureAwait(false)` on all async calls in the library.
- Implicit usings enabled.

## Test conventions

### Unit tests

- Inherit `BotTestBase` for client tests.
- Use `SetupJsonResponse(json)` or `SetupResponse(statusCode)` for mock setup.
- Async tests: `[Fact(Timeout = 5000)]` with `TestContext.Current.CancellationToken`.
- Sync tests: plain `[Fact]` (no Timeout — xUnit1069 warns when Timeout is set
  without observing `CancellationToken`).
- Use `Assert.Multiple(...)` to group related assertions on the same object.
- Assert with `HttpClientMock.Received(1).SendAsync(...)`.

### Integration tests

- Inherit `IntegrationTestBase` for HTTP tests (WireMock).
- Inherit `ReceiverIntegrationTestBase` for WebSocket/receiver tests.
- Implement `IAsyncDisposable`.
- Use `MockServer` for request/response setup.
- Async facts with appropriate timeout: `[Fact(Timeout = 15000)]` or higher.
- Constants: `BotNumber = "+491701234567"`, `RecipientNumber = "+491709876543"`.

## Commits & CI

- Conventional commits enforced by commitlint: `feat:`, `fix:`, `chore:`,
  `refactor:`, `ci:`, `docs:`.
- CI runs on PRs: `dotnet-ci.yml` (build + test) and `commitlint.yml`.
- Releases via release-please -> NuGet publish.
- Security: Trivy FS scan on dependency changes + weekly schedule.

## Tech stack

- .NET 10 (`net10.0`), C# latest
- Central Package Management (`Directory.Packages.props`)
- R3 (reactive streams), WebSocket.Rx (WebSocket client)
- xunit v3 (`xunit.v3.mtp-v2`), NSubstitute, WireMock.Net
- Source-generated System.Text.Json (`JsonBotSerializerContext`)
- SourceLink, embedded debug symbols

## References

- signal-cli REST API: https://github.com/bbernhard/signal-cli-rest-api
- signal-cli REST API swagger: https://bbernhard.github.io/signal-cli-rest-api/
