using System;
using Godot;
using SnekSweeper.CellSystem.Components;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
    #region constants

    public const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

    public const string PrimaryAction = "primary";
    public const string SecondaryAction = "secondary";

    #endregion
    
    public event Action? PrimaryReleased;
    public event Action? PrimaryDoubleClicked;
    public event Action? SecondaryReleased;

    private Content Content => GetNode<Content>("Content");
    private Area2D ClickArea => GetNode<Area2D>("ClickArea");
    
    public ICover Cover => GetNode<Cover>("Cover");
    public IFlag Flag => GetNode<Flag>("Flag");

    public override void _Ready()
    {
        ClickArea.InputEvent += OnClickAreaInputEvent;
    }

    public override void _ExitTree()
    {
        ClickArea.InputEvent -= OnClickAreaInputEvent;
    }

    private void OnClickAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event.IsActionReleased(PrimaryAction))
        {
            PrimaryReleased?.Invoke();
        }
        else if (@event is InputEventMouseButton eventMouseButton && 
                 eventMouseButton.IsAction(PrimaryAction) &&
                 eventMouseButton.DoubleClick)
        {
            PrimaryDoubleClicked?.Invoke();
        }
        else if (@event.IsActionReleased(SecondaryAction))
        {
            SecondaryReleased?.Invoke();
        }
    }

    public void SetContent(Cell cell)
    {
        if (cell.HasBomb)
        {
            Content.ShowBomb();
        }
        else
        {
            Content.ShowNeighbourBombCount(cell.NeighborBombCount);
        }
    }

    public void SetPosition((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        Position = new Vector2(j * CellSizePixels, i * CellSizePixels);
    }
}