using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.UI.GameResult;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

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
        var grid = loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus);
        var runRecorder = new GameRunRecorder(HouseKeeper.History);

        var gridStateContext = new GridStateContext(
            grid, TheGrid, runRecorder,
            this, OnLose
        );
        var gridStateMachine = new GridStateMachine(loadLevelSource, gridStateContext);
        await gridStateMachine.InitAsync(ct);

        TheGrid.Init(grid, gridStateMachine);
    }

    static void OnLose()
    {
        MessageBox.Print("Game over! Bomb revealed!");
        Autoload.SceneSwitcher.GotoSceneAsync<LosingPage>().Fire();
    }

    public Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default)
    {
        return HUD.ShowAndGetChoiceAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    public void NewGame()
    {
        Autoload.SceneSwitcher.LoadLevel(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting)).Fire();
    }

    public void BackToMainMenu()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<Main>().Fire();
    }
}