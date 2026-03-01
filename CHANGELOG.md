# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2026-02-23

### Added
- Full source code published on GitHub
- Comprehensive test suite (mediator, pipeline, notifications, DI integration)
- MIT license (replacing LGPL)
- CI/CD via GitHub Actions (build + test on push/PR, publish to NuGet on tag)
- Standalone builder API: `MonadicMediatorBuilder.Create()`
- Updated dependency to MonadicSharp 1.4.0

### Changed
- `ServiceCollectionExtensions.CreateMonadicMediatorBuilder()` now delegates to `MonadicMediatorBuilder.Create()`
- Internal `MonadicMediator` class is now properly sealed and `internal`

## [1.0.0] - 2025-10-08

### Added
- Initial release on NuGet
- `IRequest<TResult>`, `ICommand`, `ICommand<TResult>`, `IQuery<TResult>`
- `IRequestHandler<TRequest, TResult>`
- `INotification`, `INotificationHandler<TNotification>`
- `IPipelineBehavior<TRequest, TResult>` with `RequestHandlerDelegate<TResult>`
- `IMonadicMediator` with `Send` and `Publish`
- Microsoft DI integration via `AddMonadicMediator()`
- Standalone builder via `CreateMonadicMediatorBuilder()`
- Assembly scanning for auto-registration of handlers and behaviors
