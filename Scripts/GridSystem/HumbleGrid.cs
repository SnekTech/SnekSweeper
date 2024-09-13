using System.Collections.Generic;
using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GridSystem;

public partial class HumbleGrid : Node2D, IHumbleGrid
{
    [Export]
    private SkinCollection _skinCollection = null!;
    
    private readonly PackedScene _cellScene = GD.Load<PackedScene>(HumbleCell.CellScenePath);
    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private Grid _grid = null!;
    private Referee _referee = null!;

    public override void _Ready()
    {
        _grid = new Grid(this, _mainSetting.CurrentDifficulty);
        _referee = new Referee(_grid);
    }

    public override void _ExitTree()
    {
        _grid.OnDispose();
        _referee.OnDispose();
    }

    public List<IHumbleCell> InstantiateHumbleCells(int count)
    {
        var humbleCells = new List<IHumbleCell>();
        var currentSkin = _skinCollection.Skins[_mainSetting.CurrentSkinIndex];

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