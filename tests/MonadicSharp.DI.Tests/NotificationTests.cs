using FluentAssertions;
using MonadicSharp;
using MonadicSharp.DI;

namespace MonadicSharp.DI.Tests;

// ── Fixtures ───────────────────────────────────────────────────────────────────

public sealed record UserRegistered(string Email) : INotification;

public sealed class EmailHandler : INotificationHandler<UserRegistered>
{
    public List<string> SentTo { get; } = new();

    public Task<Result<Unit>> Handle(UserRegistered notification, CancellationToken ct)
    {
        SentTo.Add(notification.Email);
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}

public sealed class AuditHandler : INotificationHandler<UserRegistered>
{
    public List<string> Logged { get; } = new();

    public Task<Result<Unit>> Handle(UserRegistered notification, CancellationToken ct)
    {
        Logged.Add(notification.Email);
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}

// ── Tests ──────────────────────────────────────────────────────────────────────

public class NotificationTests
{
    [Fact]
    public async Task Publish_SingleHandler_IsInvoked()
    {
        var emailHandler = new EmailHandler();
        var mediator = MonadicMediatorBuilder.Create()
            .AddNotificationHandler(emailHandler)
            .Build();

        await mediator.Publish(new UserRegistered("alice@example.com"));

        emailHandler.SentTo.Should().ContainSingle().Which.Should().Be("alice@example.com");
    }

    [Fact]
    public async Task Publish_MultipleHandlers_AllAreInvoked()
    {
        var emailHandler = new EmailHandler();
        var auditHandler = new AuditHandler();

        var mediator = MonadicMediatorBuilder.Create()
            .AddNotificationHandler(emailHandler)
            .AddNotificationHandler(auditHandler)
            .Build();

        await mediator.Publish(new UserRegistered("bob@example.com"));

        emailHandler.SentTo.Should().ContainSingle().Which.Should().Be("bob@example.com");
        auditHandler.Logged.Should().ContainSingle().Which.Should().Be("bob@example.com");
    }

    [Fact]
    public async Task Publish_NoHandlersRegistered_CompletesWithoutError()
    {
        var mediator = MonadicMediatorBuilder.Create().Build();

        var act = async () => await mediator.Publish(new UserRegistered("nobody@example.com"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Publish_SameHandlerRegisteredTwice_CalledTwice()
    {
        var handler = new EmailHandler();

        var mediator = MonadicMediatorBuilder.Create()
            .AddNotificationHandler(handler)
            .AddNotificationHandler(handler)
            .Build();

        await mediator.Publish(new UserRegistered("x@example.com"));

        handler.SentTo.Should().HaveCount(2);
    }

    [Fact]
    public async Task Publish_NullNotification_ThrowsArgumentNullException()
    {
        var mediator = MonadicMediatorBuilder.Create().Build();

        var act = async () => await mediator.Publish(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
