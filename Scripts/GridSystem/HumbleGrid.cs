using System.Collections.Generic;
using Godot;
using SnekSweeper.CellSystem;
using SnekSweeper.GameSettings;

namespace SnekSweeper.GridSystem;

public partial class HumbleGrid : Node2D, IHumbleGrid
{
    [Export]
    private MainSetting _mainSetting = null!;
    
    private Grid _grid = null!;
    private PackedScene _cellScene = GD.Load<PackedScene>(HumbleCell.CellScenePath);

    public override void _Ready()
    {
        _grid = new Grid(this, _mainSetting.CurrentDifficulty);
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