using System;
using Godot;
using GodotUtilities;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.Constants;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.CellSystem;

[Scene]
public partial class HumbleCell : Node2D, IHumbleCell
{
    [Node] private Content content = null!;
    [Node] private Cover cover = null!;
    [Node] private Flag flag = null!;

    private const int CellSize = CoreConstants.CellSizePixels;

    public ICover Cover => cover;
    public IFlag Flag => flag;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public void SetContent(bool hasBomb, int neighborBombCount)
    {
        if (hasBomb)
        {
            content.ShowBomb();
        }
        else
        {
            content.ShowNeighbourBombCount(neighborBombCount);
        }
    }

    public void SetPosition((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        Position = new Vector2(j * CellSize, i * CellSize);
    }

    public void UseSkin(ISkin newSkin)
    {
        content.ChangeTexture(newSkin.Texture);
    }
}