using SnekSweeper.Constants;
using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;

namespace SnekSweeper.Autoloads;

public partial class SceneSwitcher : Node
{
    [Export]
    private PackedScene historyPageScene = null!;
    [Export]
    private PackedScene level1Scene = null!;
    [Export]
    private PackedScene winningScene = null!;
    [Export]
    private PackedScene losingScene = null!;

    private readonly Dictionary<SceneName, PackedScene> _scenesDict = new();

    private Node _currentScene = null!;

    public static SceneSwitcher Instance { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;

        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);

        _scenesDict[SceneName.HistoryPage] = historyPageScene;
        _scenesDict[SceneName.Level1] = level1Scene;
        _scenesDict[SceneName.Winning] = winningScene;
        _scenesDict[SceneName.Losing] = losingScene;
    }

    public void GotoScene<T>() where T : Node, ISceneScript
    {
        Callable.From(() => DeferredGotoScene(SceneFactory.Instantiate<T>()).Fire()).CallDeferred();
    }

    public void GotoScene(SceneName sceneName)
    {
        // This function will usually be called from a signal callback,
        // or some other function from the current scene.
        // Deleting the current scene at this point is
        // a bad idea, because it may still be executing code.
        // This will result in a crash or unexpected behavior.

        // The solution is to defer the load to a later time, when
        // we can be sure that no code from the current scene is running:

        Callable.From(() => DeferredGotoScene(sceneName).Fire()).CallDeferred();
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

    private async Task DeferredGotoScene(SceneName sceneName)
    {
        var rootWindow = GetTree().Root;

        var fadingMask = SceneFactory.Instantiate<FadingMask>();
        rootWindow.AddChild(fadingMask);

        await fadingMask.FadeInAsync();

        // It is now safe to remove the current scene.
        _currentScene.Free();

        var nextScene = _scenesDict[sceneName];

        // add the new scene to root
        _currentScene = nextScene.Instantiate();
        rootWindow.AddChild(_currentScene);

        await fadingMask.FadeOutAsync();
        fadingMask.QueueFree();


        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = _currentScene;
    }
}