using System;
using Godot;
using GodotUtilities;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.Constants;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.CellSystem;

[Scene]
public partial class HumbleCell : Node2D, IHumbleCell
{
    private const int CellSizePixels = 16;

    public event Action? PrimaryReleased;
    public event Action? PrimaryDoubleClicked;
    public event Action? SecondaryReleased;

    [Node] private Content content = default!;
    [Node] private Area2D clickArea = default!;
    [Node] private Cover cover = default!;
    [Node] private Flag flag = default!;

    public ICover Cover => cover;
    public IFlag Flag => flag;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        clickArea.InputEvent += OnClickAreaInputEvent;
    }

    public override void _ExitTree()
    {
        clickArea.InputEvent -= OnClickAreaInputEvent;
    }

    private void OnClickAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event.IsActionReleased(InputActions.Primary))
        {
            PrimaryReleased?.Invoke();
        }
        else if (@event is InputEventMouseButton eventMouseButton &&
                 eventMouseButton.IsAction(InputActions.Primary) &&
                 eventMouseButton.DoubleClick)
        {
            PrimaryDoubleClicked?.Invoke();
        }
        else if (@event.IsActionReleased(InputActions.Secondary))
        {
            SecondaryReleased?.Invoke();
        }
    }

    public void SetContent(bool hasBomb, int neighborBombCount)
    {
        if (hasBomb)
        {
            content.ShowBomb();
        }
        else
        {
            content.ShowNeighbourBombCount(neighborBombCount);
        }
    }

    public void SetPosition((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        Position = new Vector2(j * CellSizePixels, i * CellSizePixels);
    }

    public void UseSkin(ISkin newSkin)
    {
        content.ChangeTexture(newSkin.Texture);
    }
}