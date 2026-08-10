---
name: godot-csharp-patterns
description: >
  Use when: writing Godot C# scene scripts, implementing Core interfaces in
  Godot, creating Godot scenes, wiring Core logic to Godot presentation
  layer, adding animations or shader effects, or structuring Godot C# project
  architecture. Applies the POCO+interface pattern where Core defines
  abstractions and Godot scene scripts implement them as the presentation
  layer.
---

# Godot C# Patterns

## Architecture: Core + Godot Layer

```
src/
  MyProjectCore/        ← Pure logic, interfaces, records (no Godot)
    IFoo.cs             ← Core defines: public interface IFoo { ... }
    FooData.cs          ← Core defines: public readonly record struct FooData(...)
  MyProject/            ← Godot C# project
    Scripts/
      Foo/
        HumbleFoo.cs    ← [SceneTree] partial class HumbleFoo : Node2D, IFoo, ISceneScript
        HumbleFoo.tscn
```

**Golden rule**: Core knows nothing about Godot. Godot knows about Core and implements its interfaces.

## Core Interface → Godot Implementation

Core defines a plain C# interface with domain data as records:

```csharp
// --- MyProjectCore (no Godot dependency) ---
namespace MyProjectCore.Foo;

public interface IFoo
{
    void Display(FooData data);
    Task PlayIntroAsync(CancellationToken ct = default);
}

public readonly record struct FooData(string Label, int Value);
```

Godot project implements it as a scene script. The recommended approach uses
`[SceneTree]` + `ISceneScript` for type-safe, source-generated node access:

```csharp
// --- MyProject Godot layer ---
using GDTask;
using GTweens.Extensions;
using MyProjectCore.Foo;

namespace MyProject.Foo;

[SceneTree]
public partial class HumbleFoo : Node2D, IFoo, ISceneScript
{
    const float AnimationDuration = 0.3f;

    public void Display(FooData data)
    {
        _.Label.Text = data.Label;
        _.ValueDisplay.Text = data.Value.ToString();
    }

    public async Task PlayIntroAsync(CancellationToken ct = default)
    {
        var tween = GTweenExtensions.Tween(
            () => _.Sprite.Position.Y,
            y => _.Sprite.Position = _.Sprite.Position with { Y = y },
            targetValue: 0,
            AnimationDuration
        );
        await tween.PlayAsyncUntilNodeDestroy(this, ct);
    }
}
```

## Scene Script Conventions

### [SceneTree] + ISceneScript (Recommended)

When using `[SceneTree]`, always also implement `ISceneScript`. This enables
`SceneFactory` for scene instantiation and provides source-generated typed node
access via the `_` member.

```csharp
[SceneTree]
public partial class HumbleFoo : Node2D, IFoo, ISceneScript
{
    // `_` provides typed access: _.Sprite, _.Label, _.Container, etc.
    // ISceneScript requires: static abstract string TscnFilePath { get; }
    // TscnFilePath is source-generated from the .tscn file path.
}
```

### Without [SceneTree] (Fallback for Simple Scenes)

For trivial scenes with few nodes, you can omit `[SceneTree]` and `ISceneScript`,
using manual `GetNode` or `[Export]` instead:

```csharp
public partial class HumbleFoo : Node2D, IFoo
{
    [Export] Sprite2D _sprite = null!;

    public override void _Ready()
    {
        // Manual wiring
    }
}
```

### Scene Instantiation

Use `SceneFactory` for all `[SceneTree]` + `ISceneScript` types:

```csharp
// Instantiate a scene from its .tscn path (auto-derived via ISceneScript.TscnFilePath)
var foo = HumbleFoo.Instantiate();

// Instantiate and immediately add to parent, triggering _EnterTree
var foo = HumbleFoo.InstantiateOnParent(this);
```

For non-`ISceneScript` types, fall back to:
```csharp
var foo = GD.Load<PackedScene>("res://path/to/scene.tscn").Instantiate<HumbleFoo>();
```

## Animations

Animations are a Godot-layer concern. Core never knows about tweens or frames.

- **Prefer GTweens** for tween-based animations.
- Always pass `CancellationToken` and use `PlayAsyncUntilNodeDestroy(this, ct)` so
  tweens cancel automatically if the node is freed.

```csharp
using GTweens.Extensions;

public async Task RevealAsync(CancellationToken ct = default)
{
    var tween = GTweenExtensions.Tween(
        () => _.Sprite.Modulate.A,
        a => _.Sprite.Modulate = _.Sprite.Modulate with { A = a },
        targetValue: 0f,
        duration: 0.2f
    );
    await tween.PlayAsyncUntilNodeDestroy(this, ct);
    Hide();
}
```

- Godot's built-in `Tween` is available as a fallback if GTweens is not suitable.

## Shaders

- Use `.gdshader` files in `Resources/shaders/`.
- Access shader materials via `(ShaderMaterial)GetNode<Sprite2D>("...").Material` or
  via `_` member when using `[SceneTree]`.
- Shader uniforms are a Godot-layer detail — Core should not reference shader
  parameters directly. If Core needs to control visual state, expose it through
  the interface in abstract terms (e.g., `SetAlpha(float)`, not
  `SetShaderParam(string, float)`).

## Input Handling

- Godot input events (mouse hover, click, keyboard) are captured in the Godot layer.
- Translate Godot input types (`InputEvent`, `Vector2`) into Core domain types
  before passing to Core logic.
- Core never sees `InputEvent`, `Vector2`, `GD`, or any Godot type.

```csharp
// Godot layer — convert to domain type
public override void _Input(InputEvent @event)
{
    if (@event is InputEventMouseButton { Pressed: true } mouse)
    {
        var gridIndex = ScreenToGrid(mouse.Position);  // convert Vector2 → GridIndex
        _coreService.HandleClick(gridIndex);            // Core only sees GridIndex
    }
}
```

## Asynchronous Operations

- **Prefer GDTask** for async operations in the Godot layer.
- Core uses standard `Task` / `ValueTask`. Convert at the boundary as needed.

```csharp
using GDTask;

// Fire-and-forget an async operation (e.g., in event handlers)
void OnSomeEvent() => DoSomethingAsync(CancellationToken.None).AsGDTask().Forget();

// Await multiple tasks
await GDTask.WhenAll(tasks);

// Convert standard Task to GDTask
await someTask.AsGDTask();
```

## Dependency Injection (Optional)

- Use **Chickensoft.AutoInject** if you want DI in scenes (`[Node]`, `IProvide<T>`).
- Use **Chickensoft.Introspection** + `[Meta]` for type metadata if needed.
- DI is a Godot-layer concern; Core uses plain constructor injection.
- If you don't need DI, simple manual wiring in `_Ready()` is perfectly fine.

## File Organization

```
Scripts/
  SomeDomain/
    HumbleFoo.cs           ← [SceneTree] partial class implementing Core's IFoo, ISceneScript
    HumbleFoo.tscn          ← Godot scene file
    Components/
      FooPart.cs            ← Sub-component implementing Core's IFooPart, ISceneScript
      FooPart.tscn
  Autoloads/
    SomeService.tscn        ← Godot autoload (singleton service)
  UI/
    SomeScreen/
      SomePage.cs
      SomePage.tscn
```

## What to Avoid

- ❌ Core interfaces or records referencing Godot types (`Vector2`, `Color`, `Node`,
  `GD`, `GodotObject`, etc.).
- ❌ Godot scene scripts containing business logic or domain rules — delegate to Core.
- ❌ `GD.Print()` / `GD.PrintErr()` in domain logic — use a logging abstraction
  from Core, or keep prints strictly in the Godot presentation layer for
  debugging.
- ❌ Blocking the main thread with synchronous heavy work — use async/`Task`.
- ❌ Hardcoded node paths scattered across the codebase — prefer `[SceneTree]` or
  `[Export]`-ed node references.
- ❌ Mixing `_Process` / `_PhysicsProcess` with business logic — keep them as thin
  input adapters.
- ❌ Using `[SceneTree]` without implementing `ISceneScript` — they go together.

## Scene File (`.tscn`) Editing Discipline

Scene content is authored in the Godot editor (WYSIWYG), not by agents.

- ❌ Never add, move, reparent, or restyle scene nodes, or change layout/theme
  values in `.tscn` — the user owns scene content in the editor.
- ✅ Only edit `.tscn` reference lines when a script/scene file is created,
  renamed, or deleted:
  - `ext_resource` entries (Script / PackedScene)
  - the node lines binding them (`script = ExtResource(...)`,
    `instance=ExtResource(...)`)
  - the `load_steps` count when resources are added/removed
- When deleting a script/scene, remove its `ext_resource` and its
  instance/script binding from all referencing `.tscn`/`.tres`, and delete
  companion `.uid` files.
- When renaming a `.cs`, rename its `.cs.uid` companion together so the
  `uid://` stays stable; update `path="res://..."` in references and keep
  `uid=` unchanged.
- If a refactor genuinely requires scene-structure changes, flag it as a manual
  editor step for the user instead of editing the scene.
- Verify reference edits with `dotnet build` and by grepping for dangling
  references (deleted uids / old paths).

