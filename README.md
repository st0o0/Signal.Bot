# Signal.Bot

> A .NET Signal Messenger Bot Client - because sometimes Telegram isn't enough

[![NuGet](https://img.shields.io/nuget/v/Signal.Bot.svg)](https://www.nuget.org/packages/Signal.Bot/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/st0o0/signal.bot/build-and-release.yml?branch=main)](https://github.com/st0o0/Signal.Bot/actions)
[![License](https://img.shields.io/github/license/st0o0/Signal.Bot)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/Signal.Bot.svg)](https://www.nuget.org/packages/Signal.Bot/)


<p align="center">
     <img width="128" height="128" src="docs/logo/logo.png" alt="signal bot logo">
</p>

## Overview

**Signal.Bot** was born from a simple question: "Why is there a polished Telegram.Bot library for .NET, but nothing similar for Signal?"

If you've ever wanted to build a Signal bot in .NET but were put off by the lack of proper tooling, this library is for you. It wraps the [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) with a clean, intuitive interface that feels familiar to anyone who's used Telegram.Bot.

Whether you're building a notification system, a chat bot, or just want to automate your Signal messages - Signal.Bot makes it as straightforward as it should be.

## Why Signal.Bot?

* **Telegram.Bot-Inspired API**: If you've used Telegram.Bot, you'll feel right at home
* **Clean & Intuitive**: No wrestling with raw REST calls or JSON parsing
* **Full Feature Support**: Messages, attachments, groups, profiles - everything the Signal API offers
* **Polling Built-In**: StartReceiving method handles all the complexity of message polling
* **Production Ready**: Built for real-world use cases, not just proof-of-concepts

## Prerequisites

Before you can use Signal.Bot, you need to have the [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) running. This is the backend service that actually communicates with Signal's servers.

### Quick Setup with Docker

The easiest way to get started is using Docker:

```bash
# Create a directory for Signal configuration
mkdir -p $HOME/.local/share/signal-api

# Start the signal-cli-rest-api container
docker run -d --name signal-api --restart=always -p 8080:8080 \
  -v $HOME/.local/share/signal-api:/home/.local/share/signal-cli \
  -e 'MODE=native' \
  bbernhard/signal-cli-rest-api
```

### Register Your Signal Number

You have two options to connect your Signal number:

**Option 1: Link as Secondary Device (Recommended)**

Open `http://localhost:8080/v1/qrcodelink?device_name=signal-bot` in your browser, then:
1. Open Signal on your phone
2. Go to Settings → Linked Devices
3. Tap the + button and scan the QR code

**Option 2: Register a New Number**

```bash
# Request verification code
curl -X POST "http://localhost:8080/v1/register/+1234567890"

# Verify with the code you received via SMS
curl -X POST "http://localhost:8080/v1/register/+1234567890/verify/123456"
```

## Installation

### Package Manager

```bash
Install-Package Signal.Bot
```

### .NET CLI

```bash
dotnet add package Signal.Bot
```

### PackageReference

```xml
<PackageReference Include="Signal.Bot" Version="1.0.0" />
```

## Quick Start

Here's a simple bot that echoes back any message it receives:

```csharp
using Signal.Bot;
using Signal.Bot.Types;

// Create the bot client
var client = new SignalBotClient("http://localhost:8080");

// Your registered Signal number
var botNumber = "+1234567890";

// Start receiving messages
using var cts = new CancellationTokenSource();

await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        Console.WriteLine($"Received from {message.Source}: {message.DataMessage?.Message}");
        
        // Echo the message back
        if (!string.IsNullOrEmpty(message.DataMessage?.Message))
        {
            await botClient.SendMessageAsync(
                number: botNumber,
                message: $"You said: {message.DataMessage.Message}",
                recipients: new[] { message.Source },
                cancellationToken: ct
            );
        }
    },
    handleError: async (botClient, exception, ct) =>
    {
        Console.WriteLine($"Error: {exception.Message}");
    },
    cancellationToken: cts.Token
);

// Keep the application running
Console.WriteLine("Bot is running. Press any key to stop...");
Console.ReadKey();
cts.Cancel();
```

## Usage Examples

### Sending Messages

```csharp
var client = new SignalBotClient("http://localhost:8080");

// Simple text message
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hello from Signal.Bot!",
    recipients: new[] { "+0987654321" }
);

// Message to a group
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hello everyone!",
    groupId: "your-group-id-here"
);
```

### Sending Attachments

```csharp
// Send an image
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Check out this image!",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/path/to/image.jpg" }
);
```

### Managing Groups

```csharp
// Create a group
var group = await client.CreateGroupAsync(
    number: "+1234567890",
    name: "My Bot Group",
    members: new[] { "+1111111111", "+2222222222" }
);

// List all groups
var groups = await client.ListGroupsAsync("+1234567890");

// Update group
await client.UpdateGroupAsync(
    number: "+1234567890",
    groupId: group.Id,
    name: "Updated Group Name",
    addMembers: new[] { "+3333333333" }
);
```

### Updating Profile

```csharp
await client.UpdateProfileAsync(
    number: "+1234567890",
    name: "My Bot",
    about: "I'm a helpful Signal bot!",
    avatar: "/path/to/avatar.jpg"
);
```

### Advanced Receiving with Filtering

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Only respond to text messages
        if (message.DataMessage?.Message == null) return;
        
        var text = message.DataMessage.Message;
        var sender = message.Source;
        
        // Simple command handling
        if (text.StartsWith("/help"))
        {
            await botClient.SendMessageAsync(
                number: botNumber,
                message: "Available commands:\n/help - Show this message\n/time - Get current time",
                recipients: new[] { sender },
                cancellationToken: ct
            );
        }
        else if (text.StartsWith("/time"))
        {
            await botClient.SendMessageAsync(
                number: botNumber,
                message: $"Current time: {DateTime.Now:HH:mm:ss}",
                recipients: new[] { sender },
                cancellationToken: ct
            );
        }
    },
    handleError: async (botClient, exception, ct) =>
    {
        Console.WriteLine($"Error occurred: {exception}");
    },
    cancellationToken: cts.Token
);
```

## Features

Signal.Bot provides a complete wrapper around the Signal CLI REST API, including:

* ✅ **Sending & Receiving Messages** - Text, mentions, reactions
* ✅ **Attachments** - Images, videos, documents, voice messages
* ✅ **Groups** - Create, list, update, delete groups
* ✅ **Profiles** - Update profile name, about, avatar
* ✅ **Device Linking** - QR code and verification code support
* ✅ **Message Polling** - Built-in StartReceiving mechanism
* ✅ **Full Type Safety** - Strongly-typed models for all API responses
* ✅ **Async/Await** - Modern async patterns throughout
* ✅ **Cancellation Support** - Proper CancellationToken handling

## Architecture

Signal.Bot follows a clean, layered architecture:

```
┌─────────────────────────┐
│   Your Application      │
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│   Signal.Bot            │  ← This library
│   - SignalBotClient     │
│   - Type Models         │
│   - Extensions          │
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│  signal-cli-rest-api    │  ← Docker container
│  (HTTP REST API)        │
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│  Signal Servers         │
└─────────────────────────┘
```

## Configuration Tips

### Running Behind a Reverse Proxy

If you're running signal-cli-rest-api behind a reverse proxy:

```bash
docker run -d --name signal-api \
  -e 'SWAGGER_HOST=your-domain.com' \
  -e 'SWAGGER_USE_HTTPS_AS_PREFERRED_SCHEME=true' \
  bbernhard/signal-cli-rest-api
```

### Performance Modes

The signal-cli-rest-api supports different execution modes. For best performance with Signal.Bot:

* `MODE=native` - Best balance of speed and memory (recommended)
* `MODE=json-rpc` - Fastest but uses more memory
* `MODE=normal` - Slowest but most stable

```bash
docker run -d --name signal-api -e 'MODE=native' bbernhard/signal-cli-rest-api
```

## Troubleshooting

**Bot not receiving messages?**
- Make sure you're registered correctly
- Check that signal-cli-rest-api is running (`docker ps`)
- Verify your number can send/receive through the API: `curl http://localhost:8080/v1/about`

**"Connection refused" errors?**
- Ensure signal-cli-rest-api is accessible at the URL you provided
- Check port mapping in your Docker setup

**Messages not sending?**
- Verify recipients are valid Signal numbers
- Check logs in signal-cli-rest-api: `docker logs signal-api`

## Contributing

Contributions are welcome! This library grows with the community's needs.

### How to Contribute

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Write tests for your changes
4. Ensure all tests pass: `dotnet test`
5. Submit a Pull Request

### Contribution Guidelines

* Follow existing code style and conventions
* Include unit tests for new features
* Update documentation for API changes
* Keep changes focused and atomic

## Roadmap

Planned features for upcoming releases:

- [X] NuGet package publication
- [ ] Support for stickers and custom emojis
- [ ] Typed stories support
- [ ] Extended profile management
- [ ] Rate limiting and retry policies
- [ ] Comprehensive documentation site

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Built with ❤️ for the .NET and Signal communities**

For questions, support, or feature requests, please [open an issue](https://github.com/st0o0/signal.bot/issues).

### Related Projects

* [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) - The REST API backend (required)
* [signal-cli](https://github.com/AsamK/signal-cli) - The underlying command-line tool
* [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - Inspiration for this library's design