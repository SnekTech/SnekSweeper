using GodotTask;
using SnekSweeper.UI.Common;
using SnekSweeper.Widgets;

namespace SnekSweeper.GameStateManagement;

public partial class SceneSwitcher : Node, ISceneSwitcher
{
    [Export]
    Node CurrentSceneHolder { get; set; } = null!;

    Node _currentScene = null!;
    bool _isTransitioning;

    public override void _Ready()
    {
        _currentScene = CurrentSceneHolder.GetChild(0);
    }

    public async GDTask GotoSceneAsync<T>(Action<T>? onSceneEntered, CancellationToken ct = default)
        where T : Node, ISceneScript
    {
        // 单飞: 过渡期间忽略新请求。整个过渡期间遮罩都会挡住输入(mouse_filter 默认 Stop),
        // 所以正常操作下不会真的丢请求; 但非输入来源(定时器 / FSM 自动转移)仍可能触发,
        // 届时会出现"画面落后于状态" —— 若真发生, 再改成"排队最后一次"或"取消前一个"。
        if (_isTransitioning) return;
        _isTransitioning = true;

        var newScene = SceneFactory.Instantiate<T>();
        var fadingMask = FadingMask.InstantiateOnParent(CurrentSceneHolder);

        try
        {
            await fadingMask.FadeInAsync(ct);

            // It is now safe to remove the current scene.
            _currentScene.Free();
            // add the new scene to root
            _currentScene = newScene;
            CurrentSceneHolder.AddChild(newScene);

            onSceneEntered?.Invoke(newScene);

            await fadingMask.FadeOutAsync(ct);
        }
        finally
        {
            // finally 里同样可能"节点已死"(例如退出游戏时整棵树被拆掉), 所以先校验再释放
            if (IsInstanceValid(fadingMask)) fadingMask.QueueFree();
            _isTransitioning = false;
        }
    }
}
