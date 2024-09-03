using System.Collections.Generic;
using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GridSystem;

public partial class HumbleGrid : Node2D, IHumbleGrid
{
    [Export]
    private SkinCollection _skinCollection = null!;

    private Grid _grid = null!;
    private PackedScene _cellScene = GD.Load<PackedScene>(HumbleCell.CellScenePath);
    private MainSetting _mainSetting = HouseKeeper.MainSetting;

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
        var currentSkin = _skinCollection.SkinDict[_mainSetting.CurrentSkinName];

        for (var i = 0; i < count; i++)
        {
            var humbleCell = _cellScene.Instantiate<HumbleCell>();

            humbleCell.UseSkin(currentSkin);

            AddChild(humbleCell);
            humbleCells.Add(humbleCell);
        }

        return humbleCells;
    }
}