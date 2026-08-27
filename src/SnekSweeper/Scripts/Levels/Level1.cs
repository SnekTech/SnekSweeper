using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.GridSystem;
using SnekSweeper.GridSystem.State;
using SnekSweeper.Widgets;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using GridState = SnekSweeper.GridSystem.State.GridState;

namespace SnekSweeper.Levels;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class Level1 : Node2D,
    IProvide<LevelData>,
    ISceneScript, ILevelOrchestrator
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    [Dependency]
    IAppRepo AppRepo => this.DependOn<IAppRepo>();

    GridLogic GridLogic { get; set; } = null!;
    LogicBlock.Binding GridBinding { get; set; } = null!;

    LevelData _levelData = null!;
    LevelData IProvide<LevelData>.Value() => _levelData;

    public override void _EnterTree()
    {
        _levelData = new LevelData(new GridEventBus(), new CommandInvoker());
        this.Provide();

        TheGrid.GridInputListener.GridInputEmitted += OnGridInputEmitted;
    }

    public override void _ExitTree()
    {
        SaveData.NotifySaved();

        TheGrid.GridInputListener.GridInputEmitted -= OnGridInputEmitted;

        GridLogic.Stop();
        GridBinding.Dispose();
    }

    public GDTask LoadLevel(LoadLevelSource loadLevelSource)
    {
        var grid = CreateGrid();
        TheGrid.Init(grid.Size);

        SetupGridLogic();
        SetupGridBinding();

        GridLogic.Start<GridState.PreInstantiated>();
        GridLogic.Input(new GridState.Input.Init(loadLevelSource));

        return GDTask.CompletedTask;

        Grid CreateGrid()
        {
            var gridSkin = AppRepo.CurrentSkin;
            TheGrid.HumbleCellsContainer.Clear(); // 建图前清空演示容器（集合角色，Godot 层驱动）
            return loadLevelSource.CreateGrid(TheGrid.CellFactory, gridSkin, _levelData.GridEventBus,
                _levelData.GridCommandInvoker);
        }

        void SetupGridLogic()
        {
            GridLogic = new GridLogic();

            GridLogic.Set(new GridLogic.Data
            {
                AppRepo = AppRepo,
                CancellationTokenOnLevelExit = this.GetCancellationTokenOnTreeExit(),
            });

            GridLogic.Set(new GridStateContext(
                grid,
                TheGrid,
                new GameRunRecorder(
                    () => SaveData.CurrentRunInfo,
                    SaveData.UpdateCurrentRunInfo,
                    SaveData.UpdateHistory),
                this
            ));
        }

        void SetupGridBinding()
        {
            GridBinding = GridLogic.Bind()
                .OnOutput((in GridState.Output.RestoreGrid output) =>
                {
                    RestoreGridAsync(output.Source).Forget();
                })
                .OnOutput((in GridState.Output.LayMinesAt output) =>
                {
                    LayMinesAsync(output.Source, output.FirstInput).Forget();
                })
                .OnOutput((in GridState.Output.ProcessInput output) =>
                {
                    HandleInputAsync(output.GridInput).Forget();
                })
                .OnOutput((in GridState.Output.EndGameChoiceOnWin output) =>
                {
                    Action handleChoiceAction = output.Choice switch
                    {
                        PopupChoiceOnWin.NewGame => NewGame,
                        PopupChoiceOnWin.Leave => BackToMainMenu,
                        _ => delegate { },
                    };
                    handleChoiceAction();
                })
                .OnOutput((in GridState.Output.EndGameChoiceOnLose output) =>
                {
                    var recentRecord = output.RecentRecord;
                    Action handleChoiceAction = output.Choice switch
                    {
                        PopupChoiceOnLose.Retry => () => Retry(recentRecord),
                        PopupChoiceOnLose.NewGame => NewGame,
                        PopupChoiceOnLose.Leave => BackToMainMenu,
                        _ => delegate { },
                    };
                    handleChoiceAction();
                });

            return;

            // 初始化放在绑定层：续局恢复完整棋盘状态，新局/重试按首次点击布雷
            async GDTaskVoid RestoreGridAsync(FromGridSnapshot source)
            {
                await grid.InitCellsAsync(source.Snapshot, this.GetCancellationTokenOnTreeExit());
                CompleteInit();
            }

            async GDTaskVoid LayMinesAsync(LoadLevelSource source, GridInput firstInput)
            {
                await grid.InitCellsAsync(source.LayMineFn(firstInput.Index), this.GetCancellationTokenOnTreeExit());
                CompleteInit();
            }

            void CompleteInit() => GridLogic.Input(new GridState.Input.InitCompleted());

            // 异步输入处理放在绑定层：FSM 只发效果、不 await，完成后以 InputProcessed 回调回 FSM
            async GDTaskVoid HandleInputAsync(GridInput gridInput)
            {
                var processResult = await grid.HandleInputAsync(gridInput, this.GetCancellationTokenOnTreeExit());
                GridLogic.Input(new GridState.Input.InputProcessed(processResult));
            }
        }
    }

    public async Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnWinAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    public async Task<PopupChoiceOnLose> GetPopupChoiceOnLoseAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnLoseAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    void OnGridInputEmitted(GridInput input)
    {
        GridLogic.Input(new GridState.Input.PlayerInput(input));
    }

    public void NewGame()
    {
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(SaveData.MainSetting));
    }

    public void BackToMainMenu()
    {
        AppLogic.Input(new AppState.Input.BackToMainMenu());
    }

    public void Retry(GameRunRecord runRecord)
    {
        AppLogic.InputNewGame(new FromRunRecord(runRecord));
    }
}
