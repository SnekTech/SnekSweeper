using GodotTask;
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
        // 单飞: 换场期间忽略新请求。换场目前只占一帧, 所以只有"同一帧内被触发两次"才命中;
        // 非输入来源(定时器 / FSM 自动转移)若真造成"画面落后于状态", 再改成"排队最后一次"或"取消前一个"。
        if (_isTransitioning) return;
        _isTransitioning = true;

        try
        {
            var newScene = SceneFactory.Instantiate<T>();

            // Free() 是同步销毁: 节点的 _ExitTree / Dispose 在那一行返回前就跑完了, 不存在"等旧场景退场"。
            // 需要这一帧的是另一件事 —— 换场是从旧场景自己的按钮按下那条链上发出来的,
            // 不能在它的输入/信号回调里把它拆掉(这也是 QueueFree() 存在的原因);
            // 这个帧边界的续体在调用栈解开之后才跑, 就是那道保护。
            // (将来旧场景有了退场效果, 这里会变成 await 那个退场动画。)
            await GDTask.Yield();

            _currentScene.Free();
            _currentScene = newScene;
            CurrentSceneHolder.AddChild(newScene);

            onSceneEntered?.Invoke(newScene);
        }
        finally
        {
            // onSceneEntered 可能抛(例如关卡初始化), 抛了也必须把单飞标志放回去,
            // 否则之后所有换场都会被静默忽略
            _isTransitioning = false;
        }
    }
}
