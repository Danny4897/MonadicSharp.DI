using FluentAssertions;
using MonadicSharp;
using MonadicSharp.DI;

namespace MonadicSharp.DI.Tests;

// ── Fixtures ───────────────────────────────────────────────────────────────────

public sealed record PingCommand(string Message) : ICommand<string>;

public sealed class PingHandler : IRequestHandler<PingCommand, string>
{
    public Task<Result<string>> Handle(PingCommand request, CancellationToken ct)
        => Task.FromResult(Result<string>.Success($"pong:{request.Message}"));
}

public sealed class OrderTrackingBehavior : IPipelineBehavior<PingCommand, string>
{
    private readonly List<string> _log;
    private readonly string _name;

    public OrderTrackingBehavior(List<string> log, string name)
    {
        _log = log;
        _name = name;
    }

    public async Task<Result<string>> Handle(PingCommand request, CancellationToken ct, RequestHandlerDelegate<string> next)
    {
        _log.Add($"{_name}:before");
        var result = await next();
        _log.Add($"{_name}:after");
        return result;
    }
}

public sealed class ShortCircuitBehavior : IPipelineBehavior<PingCommand, string>
{
    private readonly string _failureMessage;

    public ShortCircuitBehavior(string failureMessage) => _failureMessage = failureMessage;

    public Task<Result<string>> Handle(PingCommand request, CancellationToken ct, RequestHandlerDelegate<string> next)
        => Task.FromResult(Result<string>.Failure(_failureMessage));
}

public sealed class MutatingBehavior : IPipelineBehavior<PingCommand, string>
{
    public async Task<Result<string>> Handle(PingCommand request, CancellationToken ct, RequestHandlerDelegate<string> next)
    {
        var result = await next();
        return result.Map(v => v.ToUpperInvariant());
    }
}

// ── Tests ──────────────────────────────────────────────────────────────────────

public class PipelineTests
{
    [Fact]
    public async Task SingleBehavior_ExecutesAroundHandler()
    {
        var log = new List<string>();

        var mediator = MonadicMediatorBuilder.Create()
            .AddBehavior(new OrderTrackingBehavior(log, "B1"))
            .AddHandler(new PingHandler())
            .Build();

        var result = await mediator.Send(new PingCommand("hello"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:hello");
        log.Should().ContainInOrder("B1:before", "B1:after");
    }

    [Fact]
    public async Task MultipleBehaviors_ExecuteOutermostFirst()
    {
        var log = new List<string>();

        var mediator = MonadicMediatorBuilder.Create()
            .AddBehavior(new OrderTrackingBehavior(log, "B1"))
            .AddBehavior(new OrderTrackingBehavior(log, "B2"))
            .AddHandler(new PingHandler())
            .Build();

        await mediator.Send(new PingCommand("test"));

        // B1 wraps B2 which wraps handler → B1:before, B2:before, B2:after, B1:after
        log.Should().ContainInOrder("B1:before", "B2:before", "B2:after", "B1:after");
    }

    [Fact]
    public async Task Behavior_CanShortCircuit_WithoutCallingNext()
    {
        var handlerCalled = false;
        var handler = new TrackingHandler(() => handlerCalled = true);

        var mediator = MonadicMediatorBuilder.Create()
            .AddBehavior(new ShortCircuitBehavior("blocked"))
            .AddHandler(handler)
            .Build();

        var result = await mediator.Send(new PingCommand("test"));

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("blocked");
        handlerCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Behavior_CanTransformResult()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddBehavior(new MutatingBehavior())
            .AddHandler(new PingHandler())
            .Build();

        var result = await mediator.Send(new PingCommand("test"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("PONG:TEST");
    }

    [Fact]
    public async Task NoBehaviors_HandlerExecutesDirectly()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(new PingHandler())
            .Build();

        var result = await mediator.Send(new PingCommand("direct"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:direct");
    }

    private sealed class TrackingHandler : IRequestHandler<PingCommand, string>
    {
        private readonly Action _onHandle;
        public TrackingHandler(Action onHandle) => _onHandle = onHandle;

        public Task<Result<string>> Handle(PingCommand request, CancellationToken ct)
        {
            _onHandle();
            return Task.FromResult(Result<string>.Success("ok"));
        }
    }
}
