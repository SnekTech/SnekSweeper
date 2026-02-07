using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.UI.GameResult;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript
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
            OnWin, OnLose
        );
        var gridStateMachine = new GridStateMachine(loadLevelSource, gridStateContext);
        await gridStateMachine.InitAsync(ct);

        TheGrid.Init(grid, gridStateMachine);
    }

    static void OnWin()
    {
        MessageBox.Print("You win!");
        Autoload.SceneSwitcher.GotoSceneAsync<WinningPage>().Fire();
    }

    static void OnLose()
    {
        MessageBox.Print("Game over! Bomb revealed!");
        Autoload.SceneSwitcher.GotoSceneAsync<LosingPage>().Fire();
    }
}