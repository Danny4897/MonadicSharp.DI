namespace MonadicSharp.DI;

/// <summary>Marker interface for notifications (events published to multiple handlers).</summary>
public interface INotification { }

/// <summary>Handles a specific notification type.</summary>
public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task<Result<Unit>> Handle(TNotification notification, CancellationToken ct);
}
