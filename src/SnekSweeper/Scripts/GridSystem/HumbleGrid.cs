using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;
using SnekSweeper.UI;
using SnekSweeper.Widgets;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    [Export] private SkinCollection skinCollection = null!;
    [Export] private PackedScene cellScene = null!;

    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private Grid _grid = null!;
    private Referee _referee = null!;
    private readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    public CommandInvoker GridCommandInvoker { get; } = new();

    public override void _Ready()
    {
        var bombMatrix = new BombMatrix(_mainSetting.CurrentDifficulty);
        _grid = new Grid(this, bombMatrix);
        _referee = new Referee(_grid);
    }

    public override void _EnterTree()
    {
        _hudEventBus.UndoPressed += OnUndoPressed;
        GridInputListener.PrimaryReleased += OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked += OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased += OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _referee.OnDispose();

        _hudEventBus.UndoPressed -= OnUndoPressed;
        GridInputListener.PrimaryReleased -= OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked -= OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased -= OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
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

    private void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        var shouldShowCursor = _grid.IsValidIndex(hoveringGridIndex);
        if (!shouldShowCursor)
        {
            Cursor.Hide();
            return;
        }

        Cursor.ShowAtHoveringCell(hoveringGridIndex);
    }

    private void OnUndoPressed() => GridCommandInvoker.UndoCommandAsync().Fire();

    private void OnPrimaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryReleasedAt(gridIndex).Fire();
    }

    private void OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryDoubleClickedAt(gridIndex).Fire();
    }

    private void OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnSecondaryReleasedAt(gridIndex).Fire();
    }
}