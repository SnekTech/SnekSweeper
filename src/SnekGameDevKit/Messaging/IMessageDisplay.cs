namespace SnekGameDevKit.Messaging;

/// <summary>
/// 消息显示抽象。fire-and-forget（异步显示 + 异常处理）由实现层负责，
/// 纯 .NET 层只做同步转发。
/// </summary>
public interface IMessageDisplay
{
    void Fire(string message);
}
