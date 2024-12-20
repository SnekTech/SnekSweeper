using System;
using System.Collections.Generic;
using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Constants;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GridSystem;

public partial class HumbleGrid : Node2D, IHumbleGrid
{
    public event Action<(int i, int j)>? PrimaryReleased;
    public event Action<(int i, int j)>? PrimaryDoubleClicked;
    public event Action<(int i, int j)>? SecondaryReleased;

    [Export] private SkinCollection skinCollection = null!;
    [Export] private PackedScene cellScene = null!;
    [Export] private GridCursor cursor = null!;

    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private const int CellSize = CoreStats.CellSizePixels;
    private Grid _grid = null!;
    private Referee _referee = null!;
    private (int i, int j) _hoveringGridIndex = (0, 0);

    public override void _Ready()
    {
        _grid = new Grid(this, _mainSetting.CurrentDifficulty);
        _referee = new Referee(_grid);
        cursor.Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouse) return;

        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            var newHoveringGridIndex = GetHoveringGridIndex(eventMouseMotion.Position);
            var hoveringGridIndexHasChanged = newHoveringGridIndex != _hoveringGridIndex;
            if (!hoveringGridIndexHasChanged)
                return;

            _hoveringGridIndex = newHoveringGridIndex;
            UpdateCursor();
        }
        else if (@event.IsActionReleased(InputActions.Primary))
        {
            PrimaryReleased?.Invoke(_hoveringGridIndex);
        }
        else if (@event is InputEventMouseButton eventMouseButton &&
                 eventMouseButton.IsAction(InputActions.Primary) &&
                 eventMouseButton.DoubleClick)
        {
            PrimaryDoubleClicked?.Invoke(_hoveringGridIndex);
        }
        else if (@event.IsActionReleased(InputActions.Secondary))
        {
            SecondaryReleased?.Invoke(_hoveringGridIndex);
        }
    }

    public override void _ExitTree()
    {
        _grid.OnDispose();
        _referee.OnDispose();
    }

    public List<IHumbleCell> InstantiateHumbleCells(int count)
    {
        var humbleCells = new List<IHumbleCell>();
        var currentSkin = skinCollection.Skins[_mainSetting.CurrentSkinIndex];

        for (var i = 0; i < count; i++)
        {
            var humbleCell = cellScene.Instantiate<HumbleCell>();

            humbleCell.UseSkin(currentSkin);

            AddChild(humbleCell);
            humbleCells.Add(humbleCell);
        }

        return humbleCells;
    }

    private (int i, int j) GetHoveringGridIndex(Vector2 globalMousePosition)
    {
        var localMousePosition = globalMousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / CellSize);
        var j = Mathf.FloorToInt(localMousePosition.X / CellSize);
        return (i, j);
    }

    private void UpdateCursor()
    {
        var shouldShowCursor = _grid.IsValidIndex(_hoveringGridIndex);
        if (!shouldShowCursor)
        {
            cursor.Hide();
            return;
        }

        cursor.ShowAtHoveringCell(_hoveringGridIndex);
    }
}