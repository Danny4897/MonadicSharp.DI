using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MonadicSharp.DI;

/// <summary>Extensions for registering MonadicMediator with Microsoft DI.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MonadicMediator with assembly scanning for handlers and behaviors.
    /// </summary>
    /// <example>
    /// <code>
    /// services.AddMonadicMediator(opts =>
    /// {
    ///     opts.Assemblies.Add(typeof(Program).Assembly);
    ///     opts.EnablePipeline = true;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddMonadicMediator(
        this IServiceCollection services,
        Action<MonadicMediatorOptions>? configure = null)
    {
        var options = new MonadicMediatorOptions();
        configure?.Invoke(options);

        foreach (var assembly in options.Assemblies)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                RegisterImplementationsOf(services, type, typeof(IRequestHandler<,>));
                RegisterImplementationsOf(services, type, typeof(INotificationHandler<>));

                if (options.EnablePipeline)
                    RegisterImplementationsOf(services, type, typeof(IPipelineBehavior<,>));
            }
        }

        // Scoped so each request gets a mediator with the correct scoped IServiceProvider,
        // enabling resolution of scoped dependencies (e.g. EF Core repositories).
        services.AddScoped<IMonadicMediator>(sp => new MonadicMediator(sp));
        return services;
    }

    /// <summary>Creates a standalone builder that does not require a DI container.</summary>
    public static MonadicMediatorBuilder CreateMonadicMediatorBuilder()
        => MonadicMediatorBuilder.Create();

    private static void RegisterImplementationsOf(
        IServiceCollection services, Type implementationType, Type openGenericInterface)
    {
        var interfaces = implementationType
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface);

        foreach (var iface in interfaces)
        {
            // Open generic implementation (e.g. LoggingBehavior<,>): must register using
            // the generic type *definition* as both service and implementation types,
            // otherwise .NET DI throws ArgumentException at BuildServiceProvider time.
            if (implementationType.IsGenericTypeDefinition)
                services.AddTransient(openGenericInterface, implementationType);
            else
                services.AddTransient(iface, implementationType);
        }
    }
}
