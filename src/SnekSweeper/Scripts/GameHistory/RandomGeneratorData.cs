using Godot;

namespace SnekSweeper.GameHistory;

public partial class RandomGeneratorData : RefCounted
{
    [Export] public ulong Seed;
    [Export] public ulong State;
}