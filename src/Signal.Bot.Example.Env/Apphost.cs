var builder = DistributedApplication.CreateBuilder(args);

var signalCli = builder
    .AddContainer("signal-cli-rest-api", "bbernhard/signal-cli-rest-api")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("MODE", "json-rpc")
    .WithHttpEndpoint(port: 1337, targetPort: 8080, name: "http", isProxied: false)
    .WithHttpHealthCheck("v1/about", endpointName: "http")
    .WithVolume(name: "signal-data", target: "/home/.local/share/signal-cli");

var signalClient = builder
    .AddProject<Projects.Signal_Bot_Example>("signal-bot-example")
    .WaitFor(signalCli);

await builder.Build().RunAsync();