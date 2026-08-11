using SnekSweeperCore.CellSystem.StateMachine;

namespace SnekSweeperCore.CellSystem;

/// <summary>
/// Godot 层实例化一个格子单元时的产物：呈现对象 + 它持有的状态机。
/// Core 用这两者构造 <see cref="Cell"/>。
/// </summary>
public readonly record struct CellInstance(IHumbleCell HumbleCell, CellLogic Logic);
