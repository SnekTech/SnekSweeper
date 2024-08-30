using System;
using Godot;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.Constants;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
    private const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

    public event Action? PrimaryReleased;
    public event Action? PrimaryDoubleClicked;
    public event Action? SecondaryReleased;

    [Export]
    private Content _content = null!;

    [Export]
    private Area2D _clickArea = null!;

    [Export]
    private Cover _cover = null!;

    [Export]
    private Flag _flag = null!;

    public ICover Cover => _cover;
    public IFlag Flag => _flag;

    public override void _Ready()
    {
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

    public void UseSkin(ISkin newSkin)
    {
        _content.ChangeTexture(newSkin.Texture);
    }
}