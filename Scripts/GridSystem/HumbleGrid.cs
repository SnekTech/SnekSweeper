using System.Collections.Generic;
using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public partial class HumbleGrid : Node2D, IHumbleGrid
{
    private Grid _grid = null!;
    private PackedScene _cellScene = GD.Load<PackedScene>(HumbleCell.CellScenePath);

    public override void _Ready()
    {
        var mainSetting = HouseKeeper.MainSetting;
        _grid = new Grid(this, mainSetting.CurrentDifficulty);
    }

    public override void _ExitTree()
    {
        _grid.OnDispose();
    }

    public List<IHumbleCell> InstantiateHumbleCells(int count)
    {
        var humbleCells = new List<IHumbleCell>();
        for (var i = 0; i < count; i++)
        {
            var humbleCell = _cellScene.Instantiate<Node2D>();
            AddChild(humbleCell);
            humbleCells.Add((IHumbleCell)humbleCell);
        }

        return humbleCells;
    }
}