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
    [Export] private SkinCollection skinCollection = default!;

    [Export] private PackedScene cellScene = default!;

    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private Grid _grid = default!;
    private Referee _referee = default!;

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
}