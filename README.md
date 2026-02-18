# Signal.Bot

<div align="center">

<img alt="Signal.Bot logo" height="128" src="https://raw.githubusercontent.com/st0o0/Signal.Bot/eb7e4e5c69480e29a635b5f93b764aff899b3eba/docs/logo/logo.png" width="128"/>

**A .NET Signal Messenger Bot Client - because sometimes Telegram isn't enough**

[![NuGet](https://img.shields.io/nuget/v/Signal.Bot.svg?style=flat-square)](https://www.nuget.org/packages/Signal.Bot/)
[![License](https://img.shields.io/github/license/st0o0/Signal.Bot?style=flat-square)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/Signal.Bot.svg?style=flat-square)](https://www.nuget.org/packages/Signal.Bot/)
[![Deploy Documentation](https://img.shields.io/github/actions/workflow/status/st0o0/Signal.Bot/docs.yml?branch=main&style=flat-square&label=documentation)](https://st0o0.github.io/Signal.Bot/)
</div>

---

## What is Signal.Bot?

Signal.Bot wraps the [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) with a clean,
intuitive interface inspired by [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot).
If you've ever wanted to build a Signal bot in .NET but were put off by the lack of proper tooling - this is for you.

**Key highlights:**

- Telegram.Bot-inspired API — feels immediately familiar
- Built-in polling via `StartReceiving`
- Full feature support: messages, attachments, groups, profiles
- Modern async/await with proper `CancellationToken` handling

---

## Prerequisites

Signal.Bot requires [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) as a backend. The quickest
way to get it running is via Docker:

```bash
mkdir -p $HOME/.local/share/signal-api

docker run -d --name signal-api --restart=always -p 8080:8080 \
  -v $HOME/.local/share/signal-api:/home/.local/share/signal-cli \
  -e 'MODE=native' \
  bbernhard/signal-cli-rest-api
```

### Link your Signal number

**Option 1 – Link as Secondary Device (Recommended)**

Open `http://localhost:8080/v1/qrcodelink?device_name=signal-bot` in your browser, then scan the QR code via Signal →
Settings → Linked Devices.

**Option 2 – Register a New Number**

```bash
curl -X POST "http://localhost:8080/v1/register/+1234567890"
curl -X POST "http://localhost:8080/v1/register/+1234567890/verify/123456"
```

---

## Installation

```bash
# Package Manager
Install-Package Signal.Bot

# .NET CLI
dotnet add package Signal.Bot
```

```xml
<!-- PackageReference -->
<PackageReference Include="Signal.Bot" Version="1.0.0"/>
```

---

## Quick Start

```csharp
using Signal.Bot;
using Signal.Bot.Types;

var client = new SignalBotClient(builder => builder
    .WithBaseUrl("http://localhost:8080")
    .WithNumber("+1234567890"));

using var cts = new CancellationTokenSource();

client.StartReceiving(
    async (botClient, message, ct) =>
    {
        var text = message.Envelope?.DataMessage?.Message;
        Console.WriteLine($"Received from {message.Account}: {text}");

        if (!string.IsNullOrEmpty(text))
        {
            await botClient.SendMessageAsync(builder => builder
                .WithRecipient(message.Envelope!.SourceNumber!)
                .WithMessage($"You said: {text}"),
                cancellationToken: ct);
        }
    },
    async (botClient, error, ct) =>
    {
        Console.WriteLine($"Error: {error.ErrorType}: {error.Exception?.Message}");
        await Task.CompletedTask;
    },
    builder => builder.WithMaxMessages(1),
    cts.Token);

Console.WriteLine("Bot is running. Press any key to stop...");
Console.ReadKey();
cts.Cancel();
```

---

## Features

| Category        | Details                                        |
|-----------------|------------------------------------------------|
| **Messages**    | Send & receive text, mentions, reactions       |
| **Attachments** | Images, videos, documents, voice messages      |
| **Groups**      | Create, list, update, delete                   |
| **Profiles**    | Name, about, avatar                            |
| **Polling**     | Built-in `StartReceiving` mechanism            |
| **Type Safety** | Strongly-typed models throughout               |
| **Async**       | Full async/await & `CancellationToken` support |

---

## Architecture

```
Your Application
      │
      ▼
Signal.Bot          ← This library
(SignalBotClient)
      │
      ▼
signal-cli-rest-api ← Docker container
(HTTP REST API)
      │
      ▼
Signal Servers
```

---

## Configuration

### Performance Modes

Set via the `MODE` environment variable:

| Mode       | Speed   | Memory | Recommended    |
|------------|---------|--------|----------------|
| `native`   | Fast    | Low    | ✅ Yes          |
| `json-rpc` | Fastest | High   | Only if needed |
| `normal`   | Slow    | Low    | Fallback       |

---

## Troubleshooting

**Bot not receiving messages?**
Make sure you're registered and the container is running: `docker ps`. Test the API with
`curl http://localhost:8080/v1/about`.

**"Connection refused" errors?**
Check your port mapping and that the URL you configured matches the running container.

**Messages not sending?**
Verify the recipient is a valid Signal number and check container logs: `docker logs signal-api`.

---

## Contributing

1. Fork the repo and create a feature branch: `git checkout -b feature/my-feature`
2. Write tests and make sure they pass: `dotnet test`
3. Open a Pull Request

Please keep changes focused, follow the existing code style, and update docs for any API changes.

---

## License

MIT — see [LICENSE](LICENSE) for details.

---

**Built with ❤️ for the .NET and Signal communities** · [Open an issue](https://github.com/st0o0/signal.bot/issues)

### Related Projects

- [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) — Required backend
- [signal-cli](https://github.com/AsamK/signal-cli) — Underlying CLI tool
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) — Design inspiration