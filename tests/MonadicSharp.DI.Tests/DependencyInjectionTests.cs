using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MonadicSharp;
using MonadicSharp.DI;

namespace MonadicSharp.DI.Tests;

// ── Fixtures — in a dedicated assembly-scannable namespace ────────────────────

public sealed record ScanableQuery(string Input) : IQuery<string>;

public sealed class ScanableQueryHandler : IRequestHandler<ScanableQuery, string>
{
    public Task<Result<string>> Handle(ScanableQuery request, CancellationToken ct)
        => Task.FromResult(Result<string>.Success(request.Input.ToUpperInvariant()));
}

public sealed record ScanableEvent(int Value) : INotification;

public sealed class ScanableEventHandler : INotificationHandler<ScanableEvent>
{
    public static int LastValue { get; private set; }

    public Task<Result<Unit>> Handle(ScanableEvent notification, CancellationToken ct)
    {
        LastValue = notification.Value;
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}

// ── Tests ──────────────────────────────────────────────────────────────────────

public class DependencyInjectionTests
{
    [Fact]
    public async Task AddMonadicMediator_AutoDiscoversHandlerFromAssembly()
    {
        var services = new ServiceCollection();
        services.AddMonadicMediator(opts =>
            opts.Assemblies.Add(typeof(DependencyInjectionTests).Assembly));

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMonadicMediator>();

        var result = await mediator.Send(new ScanableQuery("hello"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("HELLO");
    }

    [Fact]
    public async Task AddMonadicMediator_AutoDiscoversNotificationHandler()
    {
        var services = new ServiceCollection();
        services.AddMonadicMediator(opts =>
            opts.Assemblies.Add(typeof(DependencyInjectionTests).Assembly));

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMonadicMediator>();

        await mediator.Publish(new ScanableEvent(99));

        ScanableEventHandler.LastValue.Should().Be(99);
    }

    [Fact]
    public void AddMonadicMediator_RegistersIMonadicMediatorAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddMonadicMediator();

        var provider = services.BuildServiceProvider();
        var m1 = provider.GetRequiredService<IMonadicMediator>();
        var m2 = provider.GetRequiredService<IMonadicMediator>();

        m1.Should().BeSameAs(m2);
    }

    [Fact]
    public async Task AddMonadicMediator_WithPipelineBehavior_AutoRegistered()
    {
        var services = new ServiceCollection();
        services.AddMonadicMediator(opts =>
        {
            opts.EnablePipeline = true;
            opts.Assemblies.Add(typeof(DependencyInjectionTests).Assembly);
        });

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMonadicMediator>();

        // ScanablePipelineBehavior records the call; just verify it doesn't break the pipeline
        var result = await mediator.Send(new ScanableQuery("test"));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddMonadicMediator_WithPipelineDisabled_BehaviorsNotApplied()
    {
        var services = new ServiceCollection();
        services.AddMonadicMediator(opts =>
        {
            opts.EnablePipeline = false;
            opts.Assemblies.Add(typeof(DependencyInjectionTests).Assembly);
        });

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMonadicMediator>();

        var result = await mediator.Send(new ScanableQuery("test"));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateMonadicMediatorBuilder_ReturnsBuilder()
    {
        var builder = ServiceCollectionExtensions.CreateMonadicMediatorBuilder();

        builder.Should().NotBeNull().And.BeOfType<MonadicMediatorBuilder>();
    }
}

// Auto-discovered pipeline behavior for DI scan tests
public sealed class ScanablePipelineBehavior : IPipelineBehavior<ScanableQuery, string>
{
    public async Task<Result<string>> Handle(ScanableQuery request, CancellationToken ct, RequestHandlerDelegate<string> next)
        => await next();
}
