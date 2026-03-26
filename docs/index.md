---
layout: home

hero:
  name: "MonadicSharp.DI"
  text: "Functional mediator for .NET"
  tagline: "CQRS aligned with MonadicSharp primitives. Handlers return Result<T> — not exceptions. Pipeline behaviors that compose cleanly."
  actions:
    - theme: brand
      text: Get Started
      link: /getting-started
    - theme: alt
      text: CQRS Pattern
      link: /cqrs
    - theme: alt
      text: GitHub
      link: https://github.com/Danny4897/MonadicSharp.DI

features:
  - icon: 📋
    title: Query Handlers
    details: IQueryHandler<TQuery, TResult> returns Result<TResult>. Queries that can fail do so explicitly — no null returns, no thrown InvalidOperationException.
    link: /api/query-handler
    linkText: Query handler docs

  - icon: ✏️
    title: Command Handlers
    details: ICommandHandler<TCommand, TResult> for write operations. Commands compose with validation pipeline behaviors before execution.
    link: /api/command-handler
    linkText: Command handler docs

  - icon: 🔗
    title: Pipeline Behaviors
    details: Cross-cutting concerns (validation, logging, caching, authorization) as composable IPipelineBehavior<TRequest, TResult> — applied in declaration order.
    link: /pipeline-behaviors
    linkText: Pipeline behaviors

  - icon: 📢
    title: Notifications (Pub/Sub)
    details: INotification for fire-and-forget events. Multiple handlers per notification, no coupling between publishers and subscribers.
    link: /api/notification
    linkText: Notifications docs

  - icon: 💉
    title: DI-first
    details: Works out of the box with Microsoft.Extensions.DependencyInjection. Handlers are discovered and registered automatically — no manual mapping required.
    link: /getting-started
    linkText: Setup guide

  - icon: 🪶
    title: Zero overhead
    details: Standalone mode with no DI container required. Single assembly, no reflection at request time, no code generation — just an interface and a dispatcher.
---
