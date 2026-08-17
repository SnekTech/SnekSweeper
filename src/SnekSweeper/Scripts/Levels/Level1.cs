using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
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
        HouseKeeper.TriggerPlayerDataSave();

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
                    () => HouseKeeper.CurrentRunInfo,
                    HouseKeeper.UpdateCurrentRunInfo,
                    HouseKeeper.UpdateHistory),
                this
            ));
        }

        void SetupGridBinding()
        {
            GridBinding = GridLogic.Bind()
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
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting));
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
