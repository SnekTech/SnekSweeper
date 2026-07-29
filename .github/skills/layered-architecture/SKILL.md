---
name: layered-architecture
description: >
  Use when: designing a new Godot C# component or feature, structuring logic
  across files, deciding where computation vs rendering belongs, extracting
  reusable code into a classlib, or refactoring a scene script that mixes
  decisions with engine calls. Applies a three-layer architecture — pure
  logic, Godot-aware execution, and scene scripts — regardless of whether
  the layers live in a shared classlib or within a single Godot project.
---

# Layered Architecture

## Three Layers

```
Layer 1: Pure Logic
  ├── Zero references to Godot, GTweens, or any engine types.
  ├── Defines interfaces (IScrollMenuItem, IHumbleCell, ICover) as contracts.
  ├── Contains all decision-making: state machines, navigation, computation.
  ├── Outputs DTOs (records) that describe WHAT should happen.
  └── Fully unit-testable without a Godot process.

Layer 2: Godot-aware POCO
  ├── References GodotSharp + GTweens (classlib csproj permits this).
  ├── Takes Layer 1 DTOs and EXECUTES them on Godot types (Control, Node).
  ├── Does NOT make decisions — only translates commands into engine calls.
  ├── May hold Godot object references (Control, Material) as internal state.
  └── Typically sealed classes with constructor injection of Layer 1 components.

Layer 3: Godot Scene Script
  ├── Thin skeletons: instantiate Layer 1 + Layer 2, wire events.
  ├── Handle input, parenting, and scene lifecycle (_Ready, _ExitTree).
  ├── Implement Layer 1 interfaces on Godot nodes.
  └── One scene script per concrete usage — not a reusable component.
```

## Decision Rule

| If the code... | It belongs in... |
|---|---|
| Computes something from pure data | Layer 1 |
| Needs a Godot type to do its job | Layer 2 |
| Responds to _Input, _Ready, scene tree events | Layer 3 |

## Project Layout

### Cross-project (shared classlib)

```
GodotGadgets/                     ← Layer 1 + 2, shared across projects
  UI/ScrollMenuCore/
    ScrollMenu.cs                 ← Layer 1
    ScrollMenuAnimator.cs         ← Layer 1
    ItemAction.cs                 ← Layer 1
    ScrollMenuViewAnimator.cs     ← Layer 2 (references GodotSharp)
  UI/Pagination/
    Pagination.cs                 ← Layer 1
    PaginationBinder.cs           ← Layer 2

SnekSweeper/                      ← Layer 3
  Scripts/UI/MainScreen/
    ScrollMenuView.cs
```

### Single-project (no classlib)

```
SnekSweeper/
  Scripts/
    SomeFeature/
      Core/                       ← Layer 1 (no Godot refs)
        FeatureLogic.cs
        FeatureConfig.cs
        FeatureDTO.cs
      Presentation/               ← Layer 2 (references GodotSharp)
        FeatureViewAnimator.cs
      FeatureView.cs              ← Layer 3
```

Same rules, same testability, different folder.

## Examples from This Project

```
Feature          Layer 1 (Core)          Layer 2 (Godot-aware)      Layer 3 (Scene)
───────          ──────────────          ─────────────────────      ───────────────
Scroll Menu      ScrollMenu              ScrollMenuViewAnimator     ScrollMenuView
                 ScrollMenuAnimator                                 MainMenuContainer
                 ScrollMenuConfig
                 ItemAction / VisibleSlot

Pagination       Pagination<T>           PaginationBinder<T>        PaginationBar
                 IPageQuery<T>                                      TutorialPage
                 IPaginationUI                                      HistoryPage

Cell System      Cell                    Cover (ICover impl)        HumbleCell
                 ICover / CoverStatus    (ShaderMaterial access)    HumbleGrid
                 IHumbleCell / IFlag

Tutorial         ExampleData             ExampleCard                TutorialPage
                 GridSnapshot                                       ExamplePagination
```

## How Layer 2 Avoids Making Decisions

Layer 2 classes are "executors," not "deciders." They receive a list of
commands from Layer 1 and apply them. Example:

```csharp
// Layer 1 outputs typed decisions
public abstract record ItemAction(IScrollMenuItem Item);
public sealed record SlideToSlot(...) : ItemAction;
public sealed record EnterFromEdge(...) : ItemAction;
public sealed record ExitToEdge(...) : ItemAction;
public sealed record StayHidden(...) : ItemAction;

// Layer 2 executes — no if-else, no business logic
public void Apply(IReadOnlyList<ItemAction> actions)
{
    foreach (var a in actions)
    {
        var node = nodeForItem[a.Item];
        switch (a)
        {
            case SlideToSlot s: TweenY(node, s.TargetY); break;
            case EnterFromEdge e: Snap(node, e.SnapFromY); TweenY(node, e.TargetY); break;
            case ExitToEdge e: TweenY(node, e.TargetY); break;
            case StayHidden: node.Modulate = ... ; break;
        }
    }
}
```

Layer 2 never asks "should this item snap?" — Layer 1 already decided.

## Anti-patterns

- ❌ Layer 1 referencing `Godot.Color`, `Vector2`, `Node`, `GD`.
- ❌ Layer 2 making domain decisions (e.g. "should this item snap?").
- ❌ Layer 3 containing tween calculation or classification logic.
- ❌ DTO carrying both semantic fields AND Godot types — pick one layer.
- ❌ Making a Layer 3 scene script "reusable" via inheritance or configuration
  when a simple Layer 2 class would do.
- ❌ Adding a nullable flag field when a polymorphic record (e.g. `SlideToSlot`
  vs `StayHidden`) eliminates the invalid state entirely.
