# Getting Started

This guide will walk you through setting up Signal.Bot and creating your first Signal bot.

## Prerequisites

Before you can use Signal.Bot, you need to have the [signal-cli-rest-api](https://github.com/bbernhard/signal-cli-rest-api) running. This is the backend service that actually communicates with Signal's servers.

## Step 1: Setup signal-cli-rest-api

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

### Verify It's Running

Open your browser and navigate to `http://localhost:8080/v1/about`. You should see the API information.

## Step 2: Register Your Signal Number

You have two options to connect your Signal number:

### Option 1: Link as Secondary Device (Recommended)

This method links your bot to an existing Signal account, similar to linking Signal Desktop.

1. Open `http://localhost:8080/v1/qrcodelink?device_name=signal-bot` in your browser
2. Open Signal on your phone
3. Go to **Settings → Linked Devices**
4. Tap the **+** button and scan the QR code displayed in your browser

::: tip
This is the recommended method as it doesn't require a separate phone number for your bot.
:::

### Option 2: Register a New Number

If you want to register a dedicated phone number for your bot:

```bash
# Request verification code
curl -X POST "http://localhost:8080/v1/register/+1234567890"

# Wait for SMS with verification code, then verify
curl -X POST "http://localhost:8080/v1/register/+1234567890/verify/123456"
```

::: warning
Replace `+1234567890` with your actual phone number in international format (including country code).
:::

## Step 3: Install Signal.Bot

Add the Signal.Bot NuGet package to your project:

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

## Step 4: Create Your First Bot

Create a new console application and add the following code:

<<< ./../../src/Signal.Bot.Example/ReadMe.cs#QuickStart{csharp}

## Step 5: Run Your Bot

```bash
dotnet run
```

Send a message to your bot's phone number from another Signal account, and you should receive an echo response!

## Next Steps

Now that you have a basic bot running, you can:

- Learn about [sending messages](/guide/sending-messages)
- Explore [receiving and handling messages](/guide/receiving-messages)
- Manage [groups](/guide/groups)
- Work with [attachments](/guide/attachments)
- Check out the [examples](/examples/)

## Troubleshooting

### Bot not receiving messages?

- Make sure you're registered correctly
- Check that signal-cli-rest-api is running: `docker ps`
- Verify your number can send/receive through the API: `curl http://localhost:8080/v1/about`

### "Connection refused" errors?

- Ensure signal-cli-rest-api is accessible at the URL you provided
- Check port mapping in your Docker setup: `docker port signal-api`

### Messages not sending?

- Verify recipients are valid Signal numbers in international format
- Check logs in signal-cli-rest-api: `docker logs signal-api`
- Make sure you're using the correct bot number

::: tip
Enable verbose logging in signal-cli-rest-api for debugging:
```bash
docker run -d --name signal-api \
  -e 'LOGGING_LEVEL=DEBUG' \
  bbernhard/signal-cli-rest-api
```
:::