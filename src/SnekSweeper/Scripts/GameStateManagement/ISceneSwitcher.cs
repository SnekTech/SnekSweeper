using GodotTask;
using SnekSweeper.Widgets;

namespace SnekSweeper.GameStateManagement;

public interface ISceneSwitcher
{
    GDTask GotoSceneAsync<T>(Action<T>? configure = null, CancellationToken ct = default) where T : Node, ISceneScript;
}