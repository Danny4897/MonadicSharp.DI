namespace MonadicSharp.DI;

/// <summary>Delegate representing the next step in the pipeline (next behavior or the handler).</summary>
public delegate Task<Result<TResult>> RequestHandlerDelegate<TResult>();

/// <summary>
/// Middleware that wraps around request handling — useful for cross-cutting concerns
/// like logging, validation, authentication, and metrics.
/// </summary>
public interface IPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<Result<TResult>> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResult> next);
}
