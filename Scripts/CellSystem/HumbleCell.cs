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
    public event Action? PrimaryReleased;
    public event Action? PrimaryDoubleClicked;
    public event Action? SecondaryReleased;

    [Node] private Content content = null!;
    [Node] private Area2D interactArea = null!;
    [Node] private Cover cover = null!;
    [Node] private Flag flag = null!;

    private const int CellSize = CoreConstants.CellSizePixels;

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
        interactArea.InputEvent += OnInteractAreaInputEvent;
    }

    public override void _ExitTree()
    {
        interactArea.InputEvent -= OnInteractAreaInputEvent;
    }

    private void OnInteractAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
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
        Position = new Vector2(j * CellSize, i * CellSize);
    }

    public void UseSkin(ISkin newSkin)
    {
        content.ChangeTexture(newSkin.Texture);
    }
}