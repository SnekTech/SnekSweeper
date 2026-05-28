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

    public GDTask LoadLevelAsync(LoadLevelSource loadLevelSource, CancellationToken ct = default)
    {
        var gridSkin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var grid = loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus, gridSkin);
        TheGrid.Init(grid);
        var runRecorder = new GameRunRecorder(HouseKeeper.CurrentRunInfo, HouseKeeper.History);

        var gridStateContext = new GridStateContext(
            grid, TheGrid, runRecorder,
            this
        );

        GridLogic = new GridLogic();
        var gridLogicData = new GridLogic.Data { AppRepo = AppRepo };
        GridLogic.Set(gridLogicData);
        GridLogic.Set(gridStateContext);

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


        GridLogic.Start();
        GridLogic.Input(new GridLogic.Input.Init(loadLevelSource));

        // todo : no need to use task here
        return GDTask.CompletedTask;
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
        GridLogic.Input(new GridLogic.Input.PlayerInput(input, this.GetCancellationTokenOnTreeExit()));
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