using System.Runtime.CompilerServices;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.GridSystem;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Chickensoft.AutoInject.Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _EnterTree()
    {
        GridInputListener.TargetChanged += OnTargetChanged;
    }

    public override void _ExitTree()
    {
        GridInputListener.TargetChanged -= OnTargetChanged;
    }

    /// <summary>尺寸是"网格多大"这一个事实，输入翻译靠它判断指针是否落在网格内。</summary>
    public void Init(GridSize gridSize) => GridInputListener.Init(gridSize);

    public IHumbleCellCollection HumbleCellsContainer => CellsContainer;
    public IGridCursor GridCursor => Cursor;

    // todo: merge with Init(size)
    public void InstantiateCells(GridSize gridSize, GridSkin skin) => CellsContainer.InstantiateCells(gridSize, skin);

    public void PlayCongratulationEffects() => CellsContainer.PlayShuffleEffect();

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(SaveData.State.ActivatedCheatCodeSet);

    public void ApplyGridOutcome(GridOutcome gridOutcome)
    {
        switch (gridOutcome)
        {
            case GridOutcome.BatchRevealed batchRevealed:
                CellsContainer.RenderSome(batchRevealed.Cells);
                break;
            case GridOutcome.BatchCovered batchCovered:
                CellsContainer.RenderSome(batchCovered.Cells);
                break;
            case GridOutcome.FlagToggled flagToggled:
                CellsContainer.Render(flagToggled.CellOutcome);
                break;
            case GridOutcome.ErrorsMarked errorsMarked:
                CellsContainer.RenderSome(errorsMarked.Cells);
                break;
            case GridOutcome.NothingHappens:
                break;
            default:
                throw new SwitchExpressionException();
        }
    }

    public void Paint(Grid grid)
    {
        foreach (var index in grid.Indices)
        {
            CellsContainer.CellAt(index).Paint(grid.InfoAt(index), grid.StateAt(index));
        }
    }

    void OnTargetChanged(PointerTarget target) => Cursor.ShowAt(target);
}