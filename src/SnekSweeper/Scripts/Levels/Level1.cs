using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.GridSystem.State;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.Levels;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class Level1 : Node2D, ISceneScript, ILevelOrchestrator
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    [Dependency]
    IAppRepo AppRepo => this.DependOn<IAppRepo>();

    GridLogic GridLogic { get; set; } = null!;
    GridLogic.IBinding GridBinding { get; set; } = null!;

    public override void _EnterTree()
    {
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
        TheGrid.Init(grid);

        SetupGridLogic();
        SetupGridBinding();

        GridLogic.Start();
        GridLogic.Input(new GridLogic.Input.Init(loadLevelSource));

        return GDTask.CompletedTask;

        Grid CreateGrid()
        {
            var gridSkin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
            return loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus, gridSkin);
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
                new GameRunRecorder(HouseKeeper.CurrentRunInfo, HouseKeeper.History),
                this
            ));
        }

        void SetupGridBinding()
        {
            GridBinding = GridLogic.Bind()
                .Handle((in GridLogic.Output.EndGameChoiceOnWin output) =>
                {
                    Action handleChoiceAction = output.Choice switch
                    {
                        PopupChoiceOnWin.NewGame => NewGame,
                        PopupChoiceOnWin.Leave => BackToMainMenu,
                        _ => delegate { },
                    };
                    handleChoiceAction();
                })
                .Handle((in GridLogic.Output.EndGameChoiceOnLose output) =>
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
        GridLogic.Input(new GridLogic.Input.PlayerInput(input));
    }

    public void NewGame()
    {
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting));
    }

    public void BackToMainMenu()
    {
        AppLogic.Input(new AppLogic.Input.BackToMainMenu());
    }

    public void Retry(GameRunRecord runRecord)
    {
        AppLogic.InputNewGame(new FromRunRecord(runRecord));
    }
}