using GodotTask;
using SnekSweeper.Widgets;

namespace SnekSweeper.GameStateManagement;

public interface ISceneSwitcher
{
    /// <summary>
    /// 换到新场景。这是一个<b>钩子</b>：调用时场景已入树、过渡尚未结束 ——
    /// 它不保证 <paramref name="onSceneEntered"/> 内部启动的流程（例如关卡展开）已经完成。
    /// </summary>
    /// <param name="onSceneEntered"></param>
    /// <param name="ct">留给"旧场景退场 / 新场景入场"的异步效果取消用；目前换场没有可取消的工作。</param>
    GDTask GotoSceneAsync<T>(Action<T>? onSceneEntered = null, CancellationToken ct = default) where T : Node, ISceneScript;
}
