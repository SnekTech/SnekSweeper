using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript, ILevelOrchestrator
{
    public override void _ExitTree()
    {
        HouseKeeper.SaveCurrentPlayerData();
    }

    public async Task LoadLevelAsync(LoadLevelSource loadLevelSource, CancellationToken ct = default)
    {
        var gridSkin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var grid = loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus, gridSkin);
        var runRecorder = new GameRunRecorder(HouseKeeper.History);

        var gridStateContext = new GridStateContext(
            grid, TheGrid, runRecorder,
            this
        );
        var gridStateMachine = new GridStateMachine(gridStateContext);
        await gridStateMachine.InitAsync(loadLevelSource, ct);

        TheGrid.Init(grid, gridStateMachine);
    }

    public Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default) =>
        HUD.ShowAndGetChoiceOnWinAsync(ct.LinkWithNodeDestroy(this).Token);

    public Task<PopupChoiceOnLose> GetPopupChoiceOnLoseAsync(CancellationToken ct = default) =>
        HUD.ShowAndGetChoiceOnLoseAsync(ct.LinkWithNodeDestroy(this).Token);

    public void NewGame()
    {
        Autoload.SceneSwitcher.LoadLevel(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting)).Fire();
    }

    public void BackToMainMenu()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<Main>().Fire();
    }

    public void Retry(GameRunRecord runRecord)
    {
        var loadLevelSource = new FromRunRecord(runRecord);
        Autoload.SceneSwitcher.LoadLevel(loadLevelSource).Fire();
    }
}