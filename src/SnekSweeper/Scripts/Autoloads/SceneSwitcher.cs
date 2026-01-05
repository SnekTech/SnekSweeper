using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;

namespace SnekSweeper.Autoloads;

public partial class SceneSwitcher : Node
{
    Node _currentScene = null!;

    public override void _Ready()
    {
        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public async Task<T> GotoSceneAsync<T>(CancellationToken cancellationToken = default) where T : Node, ISceneScript
    {
        var newScene = SceneFactory.Instantiate<T>();
        var rootWindow = GetTree().Root;

        var fadingMask = FadingMask.InstantiateOnParent(rootWindow);
        await fadingMask.FadeInAsync(cancellationToken);

        // It is now safe to remove the current scene.
        _currentScene.Free();
        // add the new scene to root
        _currentScene = newScene;
        rootWindow.AddChild(newScene);

        await fadingMask.FadeOutAsync(cancellationToken);
        fadingMask.QueueFree();

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = _currentScene;
        return newScene;
    }
}