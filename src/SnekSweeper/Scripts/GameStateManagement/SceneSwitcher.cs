using GodotTask;
using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;

namespace SnekSweeper.GameStateManagement;

public partial class SceneSwitcher : Node, ISceneSwitcher
{
    [Export]
    Node CurrentSceneHolder { get; set; } = null!;
    
    Node _currentScene = null!;

    public override void _Ready()
    {
        _currentScene = CurrentSceneHolder.GetChild(0);
    }

    public async GDTask GotoSceneAsync<T>(Func<T, GDTask> onSceneEnteredTree, CancellationToken ct = default)
        where T : Node, ISceneScript
    {
        var newScene = SceneFactory.Instantiate<T>();

        var fadingMask = FadingMask.InstantiateOnParent(CurrentSceneHolder);
        await fadingMask.FadeInAsync(ct);

        // It is now safe to remove the current scene.
        _currentScene.Free();
        // add the new scene to root
        _currentScene = newScene;
        CurrentSceneHolder.AddChild(newScene);
        await onSceneEnteredTree(newScene);

        await fadingMask.FadeOutAsync(ct);
        fadingMask.QueueFree();
    }
}