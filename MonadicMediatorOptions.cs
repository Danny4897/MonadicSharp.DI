using System.Reflection;

namespace MonadicSharp.DI;

/// <summary>Configuration options for MonadicMediator when using Microsoft DI.</summary>
public sealed class MonadicMediatorOptions
{
    /// <summary>Assemblies to scan for handlers and behaviors. Required for DI registration.</summary>
    public List<Assembly> Assemblies { get; set; } = new();

    /// <summary>When true (default), pipeline behaviors are registered and applied.</summary>
    public bool EnablePipeline { get; set; } = true;
}
