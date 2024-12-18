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
    [Export] private SkinCollection skinCollection = null!;
    [Export] private PackedScene cellScene = null!;
    [Export] private Sprite2D cursor = null!;

    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private const int CellSize = CoreConstants.CellSizePixels;
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
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            UpdateCursor(eventMouseMotion.Position);
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

    private void UpdateCursor(Vector2 globalMousePosition)
    {
        var localMousePosition = globalMousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / CellSize);
        var j = Mathf.FloorToInt(localMousePosition.X / CellSize);
        _hoveringGridIndex = (i, j);

        if (_grid.IsValidIndex(_hoveringGridIndex))
        {
            cursor.Show();
            var cellOffset = new Vector2(j * CellSize, i * CellSize);
            cursor.Position = cellOffset;
        }
        else
        {
            cursor.Hide();
        }
    }
}