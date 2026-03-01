# MonadicSharp.DI

[![NuGet](https://img.shields.io/nuget/v/MonadicSharp.DI.svg)](https://www.nuget.org/packages/MonadicSharp.DI)
[![CI](https://github.com/Danny4897/MonadicSharp.DI/actions/workflows/ci.yml/badge.svg)](https://github.com/Danny4897/MonadicSharp.DI/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A lightweight **functional mediator** for .NET — CQRS pattern aligned with [MonadicSharp](https://github.com/Danny4897/MonadicSharp) primitives.

All handlers return `Result<T>` instead of throwing exceptions. Pipeline behaviors compose cleanly. Works with or without a DI container.

## Install

```bash
dotnet add package MonadicSharp.DI
```

## Quick Start

### 1. Define a request and its handler

```csharp
public sealed record CreateUserCommand(string Email) : ICommand<Guid>;

public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(cmd.Email))
            return Task.FromResult(Result<Guid>.Failure(
                Error.Validation("Email is required", nameof(cmd.Email))));

        var id = Guid.NewGuid();
        // ... save to DB ...
        return Task.FromResult(Result<Guid>.Success(id));
    }
}
```

### 2. With Microsoft DI (ASP.NET Core)

```csharp
// Program.cs
builder.Services.AddMonadicMediator(opts =>
{
    opts.Assemblies.Add(typeof(Program).Assembly);
    opts.EnablePipeline = true;
});

// Controller / endpoint
var result = await mediator.Send(new CreateUserCommand("alice@example.com"));

result.Match(
    id   => Results.Ok(id),
    err  => Results.BadRequest(err.Message));
```

### 3. Standalone (no container)

```csharp
var mediator = MonadicMediatorBuilder.Create()
    .AddBehavior(new LoggingBehavior<CreateUserCommand, Guid>())
    .AddHandler(new CreateUserHandler(repo))
    .AddNotificationHandler(new WelcomeEmailHandler())
    .Build();

var result = await mediator.Send(new CreateUserCommand("alice@example.com"));
```

## Pipeline Behaviors

Behaviors run **outermost-first** in registration order. Each receives a `next` delegate to call the inner pipeline.

```csharp
public sealed class LoggingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<Result<TResult>> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResult> next)
    {
        var sw = Stopwatch.StartNew();
        var result = await next();
        Console.WriteLine($"{typeof(TRequest).Name} → {sw.ElapsedMilliseconds}ms  success={result.IsSuccess}");
        return result;
    }
}
```

A behavior can **short-circuit** by returning `Result<TResult>.Failure(...)` without calling `next`.

## Notifications

Publish an event to multiple handlers — all are invoked regardless of individual results.

```csharp
public sealed record UserCreated(Guid Id, string Email) : INotification;

public sealed class SendWelcomeEmail : INotificationHandler<UserCreated>
{
    public Task<Result<Unit>> Handle(UserCreated evt, CancellationToken ct)
    {
        // send email...
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}

await mediator.Publish(new UserCreated(id, "alice@example.com"));
```

## CQRS Markers

| Interface | Returns |
|-----------|---------|
| `IQuery<TResult>` | read-only, returns a value |
| `ICommand<TResult>` | writes, returns a value |
| `ICommand` | writes, returns `Unit` |

All of them implement `IRequest<TResult>` so they work transparently with `Send`.

## Requirements

- .NET 6+ or .NET 8+
- [MonadicSharp](https://www.nuget.org/packages/MonadicSharp) ≥ 1.4.0
- Microsoft.Extensions.DependencyInjection.Abstractions ≥ 8.0 (for DI mode only)

## License

MIT © [Danny4897](https://github.com/Danny4897)
