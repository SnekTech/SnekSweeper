using Godot;

namespace SnekSweeper.CheatCode;

[GlobalClass]
public partial class CheatCodeResource : Resource
{
    [Export] public string Name { get; private set; } = null!;

    [Export] public string Description { get; private set; } = null!;

    [Export] public Texture2D Icon { get; private set; } = null!;
}