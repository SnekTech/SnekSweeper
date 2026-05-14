using System.Threading.Tasks;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript, ILevelOrchestrator
{
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
        Autoload.SceneSwitcher.LoadLevel(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting)).Forget();
    }

    public void BackToMainMenu()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<Main>().Forget();
    }

    public void Retry(GameRunRecord runRecord)
    {
        var loadLevelSource = new FromRunRecord(runRecord);
        Autoload.SceneSwitcher.LoadLevel(loadLevelSource).Forget();
    }
}