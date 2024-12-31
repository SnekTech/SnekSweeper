using System.Collections.Generic;
using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;
using SnekSweeper.UI;

namespace SnekSweeper.GridSystem;

[Scene]
public partial class HumbleGrid : Node2D, IHumbleGrid
{
    [Export] private SkinCollection skinCollection = null!;
    [Export] private PackedScene cellScene = null!;

    [Node] private GridCursor cursor = null!;
    [Node] private GridInputListener gridInputListener = null!;

    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private Grid _grid = null!;
    private Referee _referee = null!;
    private readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    public IGridInputListener GridInputListener => gridInputListener;
    public CommandInvoker GridCommandInvoker { get; } = new();

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        var bombMatrix = new BombMatrix(_mainSetting.CurrentDifficulty);
        _grid = new Grid(this, bombMatrix);
        _referee = new Referee(_grid);

        _hudEventBus.UndoPressed += OnUndoPressed;
        gridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _grid.OnDispose();
        _referee.OnDispose();
        _hudEventBus.UndoPressed -= OnUndoPressed;
        gridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
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

    private void OnHoveringGridIndexChanged((int i, int j) hoveringGridIndex)
    {
        var shouldShowCursor = _grid.IsValidIndex(hoveringGridIndex);
        if (!shouldShowCursor)
        {
            cursor.Hide();
            return;
        }

        cursor.ShowAtHoveringCell(hoveringGridIndex);
    }

    private void OnUndoPressed()
    {
        GridCommandInvoker.UndoCommand();
    }
}