using System;
using Godot;

namespace SnekSweeper.CellSystem.Components;

public partial class Content : Sprite2D
{
    public void ShowBomb()
    {
        Frame = FrameIndex.Bomb;
    }

    public void ShowNeighbourBombCount(int count)
    {
        if (count is < 0 or > 8)
            throw new ArgumentOutOfRangeException(nameof(count));
        
        Frame = FrameIndex.Zero + count;
    }

    public void ChangeTexture(Texture2D newTexture)
    {
        Texture = newTexture;
    }
    
    private static class FrameIndex
    {
        public const int Bomb = 2;
        public const int Zero = 7;
    }
}