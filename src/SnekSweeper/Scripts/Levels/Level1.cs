using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
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

    public override void _ExitTree()
    {
        HouseKeeper.TriggerPlayerDataSave();
    }

    public async GDTask LoadLevelAsync(LoadLevelSource loadLevelSource, CancellationToken ct = default)
    {
        var gridSkin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var grid = loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus, gridSkin);
        var runRecorder = new GameRunRecorder(HouseKeeper.CurrentRunInfo, HouseKeeper.History);

        var gridStateContext = new GridStateContext(
            grid, TheGrid, runRecorder,
            this
        );
        var gridStateMachine = new GridStateMachine(gridStateContext);
        await gridStateMachine.InitAsync(loadLevelSource, ct);

        TheGrid.Init(grid, gridStateMachine);
    }

    public async Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnWinAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    public async Task<PopupChoiceOnLose> GetPopupChoiceOnLoseAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnLoseAsync(ct.LinkWithNodeDestroy(this).Token);
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