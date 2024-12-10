using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SnekSweeper.Constants;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class SceneManager : Node
{
    [Export] private PackedScene packedLoadingScene = null!;
    [Export] private PackedScene mainScene = null!;
    [Export] private PackedScene settingsPageScene = null!;
    [Export] private PackedScene historyPageScene = null!;
    [Export] private PackedScene level1Scene = null!;
    [Export] private PackedScene winningScene = null!;
    [Export] private PackedScene losingScene = null!;

    private readonly Dictionary<SceneName, PackedScene> _scenesDict = new();

    private Node _currentScene = null!;

    public static SceneManager Instance { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;

        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);

        _scenesDict[SceneName.Main] = mainScene;
        _scenesDict[SceneName.SettingsPage] = settingsPageScene;
        _scenesDict[SceneName.HistoryPage] = historyPageScene;
        _scenesDict[SceneName.Level1] = level1Scene;
        _scenesDict[SceneName.Winning] = winningScene;
        _scenesDict[SceneName.Losing] = losingScene;
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

        Callable.From(() => DeferredGotoScene(sceneName)).CallDeferred();
    }

    private void DeferredGotoScene(SceneName sceneName)
    {
        // It is now safe to remove the current scene.
        _currentScene.Free();


        // // show the loading scene first
        // var loadingScene = packedLoadingScene.Instantiate();
        // GetTree().Root.AddChild(loadingScene);
        // GetTree().CurrentScene = loadingScene;
        //
        // // fake loading time
        // // await Task.Delay(1000);

        var nextScene = _scenesDict[sceneName];

        // // now the new scene is ready, remove the loading scene,
        // loadingScene.Free();

        // add the new scene to root
        _currentScene = nextScene.Instantiate();
        GetTree().Root.AddChild(_currentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = _currentScene;
    }

    private static Task<PackedScene> LoadSceneAsync(string path)
    {
        return SaveLoadHelper.LoadResourceAsync<PackedScene>(path);
    }
}