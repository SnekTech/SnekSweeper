using System;
using Godot;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.Constants;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
    private const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

    public event Action? PrimaryReleased;
    public event Action? PrimaryDoubleClicked;
    public event Action? SecondaryReleased;

    private Content _content = null!;
    private Area2D _clickArea = null!;

    public ICover Cover { get; private set; } = null!;
    public IFlag Flag { get; private set; } = null!;

    public override void _Ready()
    {
        _clickArea = GetNode<Area2D>("ClickArea");
        _content = GetNode<Content>("Content");
        Cover = GetNode<Cover>("Cover");
        Flag = GetNode<Flag>("Flag");
        
        _clickArea.InputEvent += OnClickAreaInputEvent;
    }

    public override void _ExitTree()
    {
        _clickArea.InputEvent -= OnClickAreaInputEvent;
        
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

    public void SetContent(Cell cell)
    {
        if (cell.HasBomb)
        {
            _content.ShowBomb();
        }
        else
        {
            _content.ShowNeighbourBombCount(cell.NeighborBombCount);
        }
    }

    public void SetPosition((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        Position = new Vector2(j * CellSizePixels, i * CellSizePixels);
    }
}