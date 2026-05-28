using GodotTask;
using SnekSweeper.Widgets;

namespace SnekSweeper.GameStateManagement;

public interface ISceneSwitcher
{
    GDTask GotoSceneAsync<T>(Func<T, GDTask> onSceneEnteredTree, CancellationToken ct) where T : Node, ISceneScript;
    void GotoScene<T>() where T : Node, ISceneScript => GotoSceneAsync<T>(_ => GDTask.CompletedTask, CancellationToken.None).Forget();
}