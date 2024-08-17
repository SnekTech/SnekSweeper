using System;
using System.Threading.Tasks;
using Godot;
using Array = Godot.Collections.Array;

namespace SnekSweeper.SaveLoad;

public static class SaveLoadHelper
{
    public static async Task<T> LoadResourceAsync<T>(string path, int millisecondsCheckInterval = 100)
        where T : Resource
    {
        ResourceLoader.LoadThreadedRequest(path);

        var loadStatus = ResourceLoader.LoadThreadedGetStatus(path);
        var progressArray = new Array { 0 };
        while (loadStatus == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            loadStatus = ResourceLoader.LoadThreadedGetStatus(path, progressArray);
            GD.Print($"resource ( at {path} ) is being loaded, progress: {progressArray[0]}");
            await Task.Delay(millisecondsCheckInterval);
        }

        loadStatus = ResourceLoader.LoadThreadedGetStatus(path);
        if (loadStatus == ResourceLoader.ThreadLoadStatus.Failed)
        {
            var errMessage = $"Failed to load resource: {path}";
            GD.PrintErr(errMessage);
            throw new Exception(errMessage);
        }

        var packed = ResourceLoader.LoadThreadedGet(path) as T;
        return packed!;
    }
}