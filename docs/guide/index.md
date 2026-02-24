---
layout: home

hero:
  name: "Signal.Bot"
  text: "A .NET Signal Messenger Bot Client"
  tagline: Build powerful Signal bots with the simplicity of Telegram.Bot
  image:
    src: /logo.png
    alt: Signal.Bot
  actions:
    - theme: brand
      text: Get Started
      link: /guide/getting-started
    - theme: alt
      text: View on GitHub
      link: https://github.com/st0o0/Signal.Bot

features:
  - icon: 🚀
    title: Telegram.Bot-Inspired API
    details: Familiar and intuitive API design for developers who've used Telegram.Bot
  - icon: 📦
    title: Full Feature Support
    details: Messages, attachments, groups, profiles - everything the Signal API offers
  - icon: ⚡
    title: Polling Built-In
    details: StartReceiving method handles all the complexity of message polling
  - icon: 🔒
    title: Type-Safe
    details: Strongly-typed models for all API responses with full async/await support
  - icon: 🎯
    title: Production Ready
    details: Built for real-world use cases with proper error handling and cancellation support
  - icon: 🐳
    title: Easy Setup
    details: Simple Docker-based setup with signal-cli-rest-api integration
---

## Quick Example

<<< ./../../src/Signal.Bot.Example/ReadMe.cs#QuickStart{csharp}

## Why Signal.Bot?

Signal.Bot was born from a simple question: "Why is there a polished Telegram.Bot library for .NET, but nothing similar for Signal?"

If you've ever wanted to build a Signal bot in .NET but were put off by the lack of proper tooling, this library is for you. It wraps the [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) with a clean, intuitive interface that feels familiar to anyone who's used Telegram.Bot.

## Installation

::: code-group

```bash [.NET CLI]
dotnet add package Signal.Bot
```

```bash [Package Manager]
Install-Package Signal.Bot
```

```xml [PackageReference]
<PackageReference Include="Signal.Bot" Version="1.0.0" />
```

:::