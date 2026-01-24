using SnekSweeper.Levels;
using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.Autoloads;

public partial class SceneSwitcher : Node
{
    Node _currentScene = null!;

    public override void _Ready()
    {
        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public Task LoadLevel(LoadLevelSource loadLevelSource, CancellationToken ct = default) => GotoSceneAsync<Level1>(level => level.LoadLevelAsync(loadLevelSource, ct), ct);

    public async Task GotoSceneAsync<T>(Func<T, Task>? onSceneAddedToTree = null, CancellationToken ct = default)
        where T : Node, ISceneScript
    {
        var newScene = SceneFactory.Instantiate<T>();
        var rootWindow = GetTree().Root;

        var fadingMask = FadingMask.InstantiateOnParent(rootWindow);
        await fadingMask.FadeInAsync(ct);

        // It is now safe to remove the current scene.
        _currentScene.Free();
        // add the new scene to root
        _currentScene = newScene;
        rootWindow.AddChild(newScene);

        if (onSceneAddedToTree is not null)
        {
            await onSceneAddedToTree(newScene);
        }

        await fadingMask.FadeOutAsync(ct);
        fadingMask.QueueFree();

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = _currentScene;
    }
}