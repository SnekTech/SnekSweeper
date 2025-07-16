using SnekSweeper.CellSystem.Components;
using SnekSweeper.Constants;
using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;

namespace SnekSweeper.CellSystem;

[SceneTree]
public partial class HumbleCell : Node2D, IHumbleCell, ISceneScript
{
    public ICover Cover => CellCover;
    public IFlag Flag => CellFlag;

    public void SetContent(bool hasBomb, int neighborBombCount)
    {
        if (hasBomb)
        {
            Content.ShowBomb();
        }
        else
        {
            Content.ShowNeighbourBombCount(neighborBombCount);
        }
    }

    public void SetPosition(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        const int cellSize = CoreStats.CellSizePixels;
        Position = new Vector2(j * cellSize, i * cellSize);
    }

    public void UseSkin(ISkin newSkin)
    {
        Content.ChangeTexture(newSkin.Texture);
    }
}