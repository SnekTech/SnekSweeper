namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// 存档状态存储的抽象：只读 <see cref="State"/> + 唯一写入入口 <see cref="Dispatch"/>。
/// Godot 层 <c>SaveData</c> 实现；Core 层（如 <c>GameRunRecorder</c>）依赖它保持 Godot-free。
/// </summary>
public interface ISaveDataStore
{
    PlayerSaveData State { get; }
    void Dispatch(Func<PlayerSaveData, PlayerSaveData> reduce);
}
