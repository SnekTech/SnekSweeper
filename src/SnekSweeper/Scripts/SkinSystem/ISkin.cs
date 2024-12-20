using Godot;

namespace SnekSweeper.SkinSystem;

public interface ISkin
{
    string Name { get; }
    Texture2D Texture { get; }
}