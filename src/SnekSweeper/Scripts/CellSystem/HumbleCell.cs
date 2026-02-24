using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.CellSystem;

[SceneTree]
public partial class HumbleCell : Node2D, IHumbleCell, ISceneScript
{
    public const int CellSizeInPixels = 16;

    public ICover Cover => CellCover;
    public IFlag Flag => CellFlag;

    public void OnInstantiate(GridIndex gridIndex, GridSkin skin)
    {
        SetPosition(gridIndex);
        SetSkin(skin);
    }

    public void OnInit(CellInitData initData)
    {
        SetContent(initData);
    }

    public void MarkAsWrongFlagged()
    {
        _.CellCover.Hide();
        _.CellFlag.Hide();
        Content.MarkAsWrongFlagged();
    }

    public void MarkAsBombRevealed()
    {
        _.CellCover.Hide();
        _.Ground.SelfModulate = Colors.Red;
    }

    void SetContent(CellInitData initData)
    {
        if (initData.HasBomb)
        {
            Content.ShowBomb();
        }
        else
        {
            Content.ShowNeighbourBombCount(initData.NeighborBombCount);
        }
    }

    void SetPosition(GridIndex gridIndex) => Position = gridIndex.ToPosition();

    void SetSkin(GridSkin newSkin) => Content.ChangeTexture(newSkin.Texture);
}