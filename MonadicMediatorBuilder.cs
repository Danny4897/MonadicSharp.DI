namespace MonadicSharp.DI;

/// <summary>
/// Fluent builder for creating a standalone MonadicMediator without a DI container.
/// </summary>
/// <example>
/// <code>
/// var mediator = MonadicMediatorBuilder.Create()
///     .AddHandler(new CreateUserHandler(repo))
///     .AddBehavior(new LoggingBehavior&lt;CreateUserCommand, Guid&gt;())
///     .AddNotificationHandler(new WelcomeEmailHandler())
///     .Build();
/// </code>
/// </example>
public sealed class MonadicMediatorBuilder
{
    private readonly Dictionary<Type, object> _handlers = new();
    private readonly Dictionary<Type, List<object>> _notificationHandlers = new();
    private readonly List<object> _behaviors = new();

    private MonadicMediatorBuilder() { }

    /// <summary>Creates a new builder instance.</summary>
    public static MonadicMediatorBuilder Create() => new();

    /// <summary>Registers a request handler.</summary>
    public MonadicMediatorBuilder AddHandler<TRequest, TResult>(
        IRequestHandler<TRequest, TResult> handler)
        where TRequest : IRequest<TResult>
    {
        _handlers[typeof(TRequest)] = handler;
        return this;
    }

    /// <summary>Registers a notification handler.</summary>
    public MonadicMediatorBuilder AddNotificationHandler<TNotification>(
        INotificationHandler<TNotification> handler)
        where TNotification : INotification
    {
        if (!_notificationHandlers.TryGetValue(typeof(TNotification), out var list))
        {
            list = [];
            _notificationHandlers[typeof(TNotification)] = list;
        }
        list.Add(handler);
        return this;
    }

    /// <summary>Registers a pipeline behavior (added in registration order, outermost first).</summary>
    public MonadicMediatorBuilder AddBehavior<TRequest, TResult>(
        IPipelineBehavior<TRequest, TResult> behavior)
        where TRequest : IRequest<TResult>
    {
        _behaviors.Add(behavior);
        return this;
    }

    /// <summary>Builds the mediator.</summary>
    public IMonadicMediator Build()
    {
        var handlers = (IReadOnlyDictionary<Type, object>)_handlers;
        var notificationHandlers = _notificationHandlers
            .ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<object>)kvp.Value.AsReadOnly());
        var behaviors = (IReadOnlyList<object>)_behaviors.AsReadOnly();

        return new MonadicMediator(handlers, notificationHandlers, behaviors);
    }
}
