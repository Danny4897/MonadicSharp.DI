using FluentAssertions;
using MonadicSharp;
using MonadicSharp.DI;

namespace MonadicSharp.DI.Tests;

// ── Test fixtures ──────────────────────────────────────────────────────────────

public sealed record GetUserQuery(int Id) : IQuery<string>;

public sealed class GetUserHandler : IRequestHandler<GetUserQuery, string>
{
    public Task<Result<string>> Handle(GetUserQuery request, CancellationToken ct)
        => Task.FromResult(
            request.Id > 0
                ? Result<string>.Success($"User#{request.Id}")
                : Result<string>.Failure("Invalid ID"));
}

public sealed record CreateOrderCommand(string Item) : ICommand<Guid>;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
        => Task.FromResult(
            string.IsNullOrEmpty(request.Item)
                ? Result<Guid>.Failure("Item cannot be empty")
                : Result<Guid>.Success(Guid.NewGuid()));
}

public sealed record FireAndForgetCommand(string Message) : ICommand;

public sealed class FireAndForgetHandler : IRequestHandler<FireAndForgetCommand, Unit>
{
    public List<string> Received { get; } = new();

    public Task<Result<Unit>> Handle(FireAndForgetCommand request, CancellationToken ct)
    {
        Received.Add(request.Message);
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}

// ── Tests ──────────────────────────────────────────────────────────────────────

public class MediatorTests
{
    [Fact]
    public async Task Send_Query_ReturnsSuccessWithValue()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(new GetUserHandler())
            .Build();

        var result = await mediator.Send(new GetUserQuery(42));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("User#42");
    }

    [Fact]
    public async Task Send_Query_ReturnsFailureWhenHandlerFails()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(new GetUserHandler())
            .Build();

        var result = await mediator.Send(new GetUserQuery(-1));

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Invalid ID");
    }

    [Fact]
    public async Task Send_Command_ReturnsGuid()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(new CreateOrderHandler())
            .Build();

        var result = await mediator.Send(new CreateOrderCommand("Laptop"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Send_Command_FailsOnEmptyItem()
    {
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(new CreateOrderHandler())
            .Build();

        var result = await mediator.Send(new CreateOrderCommand(""));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Send_ICommand_ReturnsUnit()
    {
        var handler = new FireAndForgetHandler();
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(handler)
            .Build();

        var result = await mediator.Send(new FireAndForgetCommand("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        handler.Received.Should().ContainSingle().Which.Should().Be("ping");
    }

    [Fact]
    public async Task Send_NoHandlerRegistered_ThrowsInvalidOperationException()
    {
        var mediator = MonadicMediatorBuilder.Create().Build();

        var act = async () => await mediator.Send(new GetUserQuery(1));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*GetUserQuery*");
    }

    [Fact]
    public async Task Send_NullRequest_ThrowsArgumentNullException()
    {
        var mediator = MonadicMediatorBuilder.Create().Build();

        var act = async () => await mediator.Send<string>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Send_CancellationToken_IsForwardedToHandler()
    {
        using var cts = new CancellationTokenSource();
        CancellationToken captured = default;

        var handler = new CapturingHandler(token => captured = token);
        var mediator = MonadicMediatorBuilder.Create()
            .AddHandler(handler)
            .Build();

        await mediator.Send(new GetUserQuery(1), cts.Token);

        captured.Should().Be(cts.Token);
    }

    private sealed class CapturingHandler : IRequestHandler<GetUserQuery, string>
    {
        private readonly Action<CancellationToken> _capture;
        public CapturingHandler(Action<CancellationToken> capture) => _capture = capture;

        public Task<Result<string>> Handle(GetUserQuery request, CancellationToken ct)
        {
            _capture(ct);
            return Task.FromResult(Result<string>.Success("ok"));
        }
    }
}
