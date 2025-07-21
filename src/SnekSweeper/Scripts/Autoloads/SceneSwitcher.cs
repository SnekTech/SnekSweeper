using GodotGadgets.Extensions;
using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;

namespace SnekSweeper.Autoloads;

public partial class SceneSwitcher : Node
{
    private Node _currentScene = null!;

    public override void _Ready()
    {
        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void GotoScene<T>() where T : Node, ISceneScript
    {
        Callable.From(() => DeferredGotoScene(SceneFactory.Instantiate<T>()).Fire()).CallDeferred();
    }

    private async Task DeferredGotoScene(Node newSceneRoot)
    {
        var rootWindow = GetTree().Root;

        var fadingMask = SceneFactory.Instantiate<FadingMask>();
        rootWindow.AddChild(fadingMask);
        await fadingMask.FadeInAsync();

        // It is now safe to remove the current scene.
        _currentScene.Free();
        // add the new scene to root
        _currentScene = newSceneRoot;
        rootWindow.AddChild(newSceneRoot);

        await fadingMask.FadeOutAsync();
        fadingMask.QueueFree();

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = _currentScene;
    }
}