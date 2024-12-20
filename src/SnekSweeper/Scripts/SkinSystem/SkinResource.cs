using Godot;

namespace SnekSweeper.SkinSystem;

[GlobalClass]
public partial class SkinResource : Resource, ISkin
{
    [Export]
    public string Name { get; private set; } = null!;
    
    [Export]
    public Texture2D Texture { get; private set; } = null!;
}