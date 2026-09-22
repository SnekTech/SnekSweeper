using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.GridSystem;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
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
    public ICellFactory CellFactory => CellsContainer;
    public IGridCursor GridCursor => Cursor;

    public void PlayCongratulationEffects() => CellsContainer.PlayShuffleEffect();

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(SaveData.State.ActivatedCheatCodeSet);

    void OnTargetChanged(PointerTarget target) => Cursor.ShowAt(target);
}
