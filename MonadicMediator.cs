using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MonadicSharp.DI;

internal sealed class MonadicMediator : IMonadicMediator
{
    // DI mode
    private readonly IServiceProvider? _provider;

    // Standalone mode
    private readonly IReadOnlyDictionary<Type, object>? _handlers;
    private readonly IReadOnlyDictionary<Type, IReadOnlyList<object>>? _notificationHandlers;
    private readonly IReadOnlyList<object>? _behaviors;

    internal MonadicMediator(IServiceProvider provider)
    {
        _provider = provider;
    }

    internal MonadicMediator(
        IReadOnlyDictionary<Type, object> handlers,
        IReadOnlyDictionary<Type, IReadOnlyList<object>> notificationHandlers,
        IReadOnlyList<object> behaviors)
    {
        _handlers = handlers;
        _notificationHandlers = notificationHandlers;
        _behaviors = behaviors;
    }

    public Task<Result<TResult>> Send<TResult>(IRequest<TResult> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestType = request.GetType();

        return _provider is not null
            ? SendViaProvider<TResult>(request, requestType, ct)
            : SendViaHandlers<TResult>(request, requestType, ct);
    }

    private Task<Result<TResult>> SendViaProvider<TResult>(
        IRequest<TResult> request, Type requestType, CancellationToken ct)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResult));
        var handler = _provider!.GetRequiredService(handlerType);

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResult));
        var behaviors = _provider.GetServices(behaviorType).OfType<object>().ToList();

        return BuildAndExecutePipeline<TResult>(request, handler, behaviors, ct);
    }

    private Task<Result<TResult>> SendViaHandlers<TResult>(
        IRequest<TResult> request, Type requestType, CancellationToken ct)
    {
        if (!_handlers!.TryGetValue(requestType, out var handler))
            throw new InvalidOperationException(
                $"No handler registered for '{requestType.Name}'. " +
                $"Use AddHandler<{requestType.Name}, {typeof(TResult).Name}>() on the builder.");

        var matchingBehaviors = _behaviors!
            .Where(b => b.GetType().GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) &&
                i.GenericTypeArguments[0] == requestType &&
                i.GenericTypeArguments[1] == typeof(TResult)))
            .ToList();

        return BuildAndExecutePipeline<TResult>(request, handler, matchingBehaviors, ct);
    }

    private static Task<Result<TResult>> BuildAndExecutePipeline<TResult>(
        IRequest<TResult> request,
        object handler,
        IList<object> behaviors,
        CancellationToken ct)
    {
        RequestHandlerDelegate<TResult> pipeline = () => InvokeHandler<TResult>(handler, request, ct);

        // Wrap behaviors in reverse order so the first registered runs outermost
        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var next = pipeline;
            var behavior = behaviors[i];
            pipeline = () => InvokeBehavior<TResult>(behavior, request, ct, next);
        }

        return pipeline();
    }

    private static Task<Result<TResult>> InvokeHandler<TResult>(
        object handler, IRequest<TResult> request, CancellationToken ct)
    {
        var method = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<TResult>, TResult>.Handle))
            ?? throw new InvalidOperationException($"Handle method not found on {handler.GetType().Name}");
        return (Task<Result<TResult>>)method.Invoke(handler, [request, ct])!;
    }

    private static Task<Result<TResult>> InvokeBehavior<TResult>(
        object behavior, IRequest<TResult> request, CancellationToken ct, RequestHandlerDelegate<TResult> next)
    {
        var method = behavior.GetType().GetMethod(nameof(IPipelineBehavior<IRequest<TResult>, TResult>.Handle))
            ?? throw new InvalidOperationException($"Handle method not found on {behavior.GetType().Name}");
        return (Task<Result<TResult>>)method.Invoke(behavior, [request, ct, next])!;
    }

    public async Task Publish(INotification notification, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        var notificationType = notification.GetType();

        if (_provider is not null)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
            var handlers = _provider.GetServices(handlerType).OfType<object>().ToList();
            await InvokeNotificationHandlers(handlers, notification, ct);
        }
        else
        {
            if (_notificationHandlers!.TryGetValue(notificationType, out var handlers))
                await InvokeNotificationHandlers(handlers, notification, ct);
        }
    }

    private static async Task InvokeNotificationHandlers(
        IEnumerable<object> handlers, INotification notification, CancellationToken ct)
    {
        foreach (var handler in handlers)
        {
            var method = handler.GetType().GetMethod(nameof(INotificationHandler<INotification>.Handle))
                ?? throw new InvalidOperationException($"Handle method not found on {handler.GetType().Name}");
            await (Task<Result<Unit>>)method.Invoke(handler, [notification, ct])!;
        }
    }
}
