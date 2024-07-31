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

    public override void _Ready()
    {
        _grid = new Grid(this, _mainSetting.CurrentDifficulty);
    }

    private void RemoveAllChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }

    public List<IHumbleCell> InstantiateHumbleCells(int count)
    {
        RemoveAllChildren();

        var cellScene = GD.Load<PackedScene>(HumbleCell.CellScenePath);
        var humbleCells = new List<IHumbleCell>();
        for (var i = 0; i < count; i++)
        {
            var humbleCell = cellScene.Instantiate();
            AddChild(humbleCell);
            humbleCells.Add((IHumbleCell)humbleCell);
        }

        return humbleCells;
    }
}