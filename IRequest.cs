namespace MonadicSharp.DI;

/// <summary>Base marker interface for all requests.</summary>
public interface IRequest<TResult> { }

/// <summary>Marker interface for commands that return Unit (fire-and-forget style).</summary>
public interface ICommand : IRequest<Unit> { }

/// <summary>Marker interface for commands that return a value.</summary>
public interface ICommand<TResult> : IRequest<TResult> { }

/// <summary>Marker interface for queries.</summary>
public interface IQuery<TResult> : IRequest<TResult> { }
