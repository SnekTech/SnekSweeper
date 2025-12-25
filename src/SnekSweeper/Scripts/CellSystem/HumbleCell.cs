using SnekSweeper.CellSystem.Components;
using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;

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

    public void SetPosition(GridIndex gridIndex) => Position = gridIndex.ToPosition();

    public void UseSkin(GridSkin newSkin)
    {
        Content.ChangeTexture(newSkin.Texture);
    }
}