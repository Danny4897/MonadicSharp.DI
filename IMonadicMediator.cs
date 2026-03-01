namespace MonadicSharp.DI;

/// <summary>
/// The central mediator — dispatches requests to their handlers and publishes notifications.
/// </summary>
public interface IMonadicMediator
{
    /// <summary>Sends a request to its registered handler, passing through any pipeline behaviors.</summary>
    Task<Result<TResult>> Send<TResult>(IRequest<TResult> request, CancellationToken ct = default);

    /// <summary>Publishes a notification to all registered handlers.</summary>
    Task Publish(INotification notification, CancellationToken ct = default);
}
