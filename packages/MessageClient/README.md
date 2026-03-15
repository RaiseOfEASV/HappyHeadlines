# MessageClient

A RabbitMQ messaging wrapper built on top of [EasyNetQ](https://github.com/EasyNetQ/EasyNetQ) that provides a simple publish/subscribe API for .NET microservices.

---

## Installation

Add a project reference in your `.csproj`:

```xml
<ProjectReference Include="..\..\packages\MessageClient\MessageClient.csproj" />
```

Also add the project to your solution so the IDE can resolve it:

```bash
dotnet sln add ../../packages/MessageClient/MessageClient.csproj
```

---

## Registration

There are two overloads depending on whether you need to consume messages or only publish.

### Publish only

Registers `IMessageClient` as a singleton. No background listener is started.

```csharp
builder.Services.AddRabbitMqMessageClient(
    new RabbitMqClientOptions { ConnectionString = "amqp://guest:guest@localhost:5672/" }
);
```

### Publish + consume (with handler auto-discovery)

Registers `IMessageClient`, `MessageHandlerRegistry`, and a hosted `MessageBackgroundService` that wires up all `IMessageHandler<T>` implementations at startup via reflection.

```csharp
builder.Services.AddRabbitMqMessageClient(
    new RabbitMqClientOptions { ConnectionString = "amqp://guest:guest@localhost:5672/" },
    new MessageHandlerOptions()
);
```

---

## Connection string format

The `ConnectionString` is passed directly to EasyNetQ and follows the standard EasyNetQ format:

```
amqp://username:password@hostname:port/virtualhost
```

| Scenario | Connection string |
|----------|------------------|
| Local default | `amqp://guest:guest@localhost:5672/` |
| Docker (container-to-container) | `amqp://user:pass@rabbitmq:5672/` |
| Custom vhost | `amqp://user:pass@rabbitmq:5672/my-vhost` |

---

## Publishing a message

Inject `IMessageClient` and call `PublishAsync<T>`. The message type acts as the topic — all subscribers to the same type will receive it.

```csharp
public class CommentService(IMessageClient messageClient)
{
    public async Task CreateCommentAsync(Comment comment)
    {
        // ... persist comment ...

        await messageClient.PublishAsync(new CommentCreatedEvent
        {
            CommentId = comment.Id,
            Content   = comment.Content
        });
    }
}
```

---

## Consuming messages — `IMessageHandler<T>`

Create a class that implements `IMessageHandler<T>`. When the service is registered with `MessageHandlerOptions`, the `MessageBackgroundService` discovers all handlers at startup via reflection and subscribes them automatically — no manual wiring needed.

```csharp
public class CommentCreatedHandler : IMessageHandler<CommentCreatedEvent>
{
    public Task Handle(CommentCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received comment {message.CommentId}: {message.Content}");
        return Task.CompletedTask;
    }
}
```

> The handler is instantiated via `ActivatorUtilities`, so constructor injection of any registered services works normally.

```csharp
public class CommentCreatedHandler(ILogger<CommentCreatedHandler> logger)
    : IMessageHandler<CommentCreatedEvent>
{
    public Task Handle(CommentCreatedEvent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Comment {Id} received", message.CommentId);
        return Task.CompletedTask;
    }
}
```

---

## How auto-discovery works

When `AddRabbitMqMessageClient` is called with `MessageHandlerOptions`, the following happens at startup:

1. `MessageBackgroundService.ExecuteAsync` is called by the .NET host.
2. `MessageHandlerRegistry.RegisterAllHandlers` scans **all loaded assemblies** for concrete classes implementing `IMessageHandler<T>`.
3. For each handler found, it creates an instance and subscribes to the message type using a unique subscription ID in the format:
   ```
   {subscriptionPrefix}_{HandlerFullName}_{MessageTypeName}
   ```
4. RabbitMQ (via EasyNetQ) delivers messages of type `T` to the handler's `Handle` method.

---

## Shared contracts

Message types (events) should live in the shared `contracts` package so both producer and consumer reference the same type:

```
packages/
├── MessageClient/       # this package
└── contracts/
    ├── CommentCreatedEvent.cs
    └── ProfanityProcessedEvent.cs
```

Both the publishing service and the consuming service add a reference to `contracts`:

```xml
<ProjectReference Include="..\..\..\..\packages\contracts\contracts.csproj" />
```

---

## Limitations

- `EnqueueAsync` (work-queue pattern) is not yet implemented.
- `SubscribeAsync<T>(QueueName, handler)` is not yet implemented.
- There is no retry or dead-letter configuration exposed — EasyNetQ defaults apply.