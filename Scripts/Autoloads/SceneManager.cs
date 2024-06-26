﻿using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SnekSweeper.Constants;

namespace SnekSweeper.Autoloads;

public partial class SceneManager : Node
{
    public Node CurrentScene { get; set; }

    private PackedScene _packedLoadingScene;

    public override void _Ready()
    {
        var root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);

        // preload the loading scene
        _packedLoadingScene = GD.Load<PackedScene>(ScenePaths.LoadingScene);
    }

    public void GotoScene(string path)
    {
        // This function will usually be called from a signal callback,
        // or some other function from the current scene.
        // Deleting the current scene at this point is
        // a bad idea, because it may still be executing code.
        // This will result in a crash or unexpected behavior.

        // The solution is to defer the load to a later time, when
        // we can be sure that no code from the current scene is running:

        CallDeferred(MethodName.DeferredGotoScene, path);
    }

    private async void DeferredGotoScene(string path)
    {
        // It is now safe to remove the current scene.
        CurrentScene.Free();

        // show the loading scene first
        var loadingScene = _packedLoadingScene.Instantiate();
        GetTree().Root.AddChild(loadingScene);
        GetTree().CurrentScene = loadingScene;
        
        var nextScene = await LoadSceneAsync(path);

        // now the new scene is ready, remove the loading scene,
        loadingScene.Free();
        
        // add the new scene to root
        CurrentScene = nextScene.Instantiate();
        GetTree().Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = CurrentScene;
    }

    private static async Task<PackedScene> LoadSceneAsync(string path, int millisecondsCheckInterval = 100)
    {
        ResourceLoader.LoadThreadedRequest(path);

        var loadStatus = ResourceLoader.LoadThreadedGetStatus(path);
        var progressArray = new Array { 0 };
        while (loadStatus == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            loadStatus = ResourceLoader.LoadThreadedGetStatus(path, progressArray);
            GD.Print($"scene {path} is being loaded, progress: {progressArray[0]}");
            await Task.Delay(millisecondsCheckInterval);
        }

        loadStatus = ResourceLoader.LoadThreadedGetStatus(path);
        if (loadStatus == ResourceLoader.ThreadLoadStatus.Failed)
        {
            GD.PrintErr($"failed to load scene {path}");
            return null;
        }
        
        return ResourceLoader.LoadThreadedGet(path) as PackedScene;
    }
}