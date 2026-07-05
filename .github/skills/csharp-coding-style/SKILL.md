---
name: csharp-coding-style
description: >
  Use when: writing or generating C# code, designing C# class/struct/record
  structures, creating new C# files, refactoring C# code, or discussing C#
  architecture. Applies functional-first, immutable, framework-agnostic
  coding conventions.
---

# C# Coding Style

## Core Principles

1. **Functional-first**, but pragmatic — no monads (Result/Either/Maybe) unless there
   is a compelling, concrete reason.
2. **Stateless & immutable by default** — prefer pure functions, avoid mutation.
3. **Framework-agnostic core** — separate pure logic into a Core project using only
   POCOs (plain old C# objects) and interfaces. No framework dependencies in Core.
4. **Use modern C# features** aggressively — use the latest language version available.

## Preferred C# Features

- **Records** — `record`, `record struct`, `readonly record struct` for data.
  Use `record` for DTOs, snapshots, value objects, and messages/events.
- **Primary constructors** — on both classes and records.
- **Switch expressions** — prefer over switch statements or if-else chains.
- **Pattern matching** — `is`, `and`/`or`/`not` patterns, list patterns,
  property patterns.
- **Extension types** (`extension(T)`) — for adding domain methods without
  polluting the type itself.
- **Collection expressions** — `[]`, `[..]` syntax.
- **File-scoped namespaces**.
- **Global usings** — via `GlobalUsings.cs` in each project.
- **`required` keyword** — enforce non-null initialization at construction.
- **Nullable enabled** — `#nullable enable` project-wide.
- **Target-typed new** — `new()` when type is obvious from context.

## Project Structure Conventions

- **Core project**: Zero dependencies on external frameworks. Only .NET BCL +
  serialization/utility packages. Contains all pure domain logic.
- **Interface-driven** — Core defines abstractions via interfaces; framework-specific
  projects implement them.
- **No inheritance for logic** — use interfaces + composition; avoid deep
  inheritance hierarchies.
- **Partial classes** only when required by source generators or tooling.

## Naming & Structure

- Interfaces for abstractions: `IXxx`.
- Enums at file bottom, after the interface or class that uses them.
- One concern per file — a record, an interface, an enum plus its immediate
  extensions can coexist.
- Extension methods in `static class XxxExtensions` with `extension(TargetType)`
  syntax when possible.

## What to Avoid

- ❌ Static mutable state (statics are OK for pure functions and constants).
- ❌ Side effects in constructors or property getters.
- ❌ Framework types leaking into Core (e.g., engine-specific `Vector`, `Color`, UI types).
- ❌ Over-engineering — don't add abstraction layers "just in case." A simple
  static method is often better than a service interface.
- ❌ Mediator/CQRS/Event Sourcing unless the domain genuinely needs it.

## Code Template

```csharp
namespace MyProjectCore.SomeDomain;

public interface IMyService
{
    Task<MyResult> DoSomethingAsync(MyInput input, CancellationToken ct = default);
}

public readonly record struct MyInput(string Id, int Count);

public sealed record MyResult(bool Success, string? ErrorMessage = null);

// --- pure logic stays in Core ---
public sealed class MyService(IExternalDependency dep) : IMyService
{
    public async Task<MyResult> DoSomethingAsync(MyInput input, CancellationToken ct = default)
    {
        var data = await dep.FetchAsync(input.Id, ct);
        return data is null
            ? new MyResult(false, ErrorMessage: "Not found")
            : new MyResult(true);
    }
}
