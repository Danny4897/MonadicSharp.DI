namespace MonadicSharp.DI;

/// <summary>
/// Handles a specific request type and returns a Result-wrapped response.
/// </summary>
public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<Result<TResult>> Handle(TRequest request, CancellationToken ct);
}
