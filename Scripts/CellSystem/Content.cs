using System;
using Godot;

namespace SnekSweeper.CellSystem;

public partial class Content : Sprite2D
{
    private static class FrameIndex
    {
        public const int Bomb = 2;
        public const int Zero = 7;
    }
    
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
}