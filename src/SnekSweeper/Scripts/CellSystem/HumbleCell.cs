using System.Runtime.CompilerServices;
using GodotTask;
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

    public void SetUpAt(GridIndex index, GridSkin skin)
    {
        Position = index.ToPosition();
        Content.ChangeTexture(skin.Texture);
    }

    public void Render(CellInfo info, CellOutcome outcome)
    {
        RenderContent(info);

        switch (outcome.Event)
        {
            case CellEvent.CoverRevealed:
                Cover.RevealAsync().AsGDTask().Forget();
                break;
            case CellEvent.CoverPutOn:
                Cover.PutOnAsync().AsGDTask().Forget();
                break;
            case CellEvent.FlagRaised:
                Flag.RaiseAsync().AsGDTask().Forget();
                break;
            case CellEvent.FlagPutDown:
                Flag.PutDownAsync().AsGDTask().Forget();
                break;
            case CellEvent.RevealedBomb:
                CellCover.Hide();
                _.Ground.SelfModulate = Colors.Red;
                break;
            case CellEvent.FlagTurnedOutWrong:
                CellCover.Hide();
                CellFlag.Hide();
                Content.MarkAsWrongFlagged();
                break;
            case null:
                ResetVisuals();
                break;
            default:
                throw new SwitchExpressionException();
        }
    }

    void RenderContent(CellInfo info)
    {
        if (info.HasBomb)
        {
            Content.ShowBomb();
        }
        else
        {
            Content.ShowNeighbourBombCount(info.NeighborBombCount);
        }
    }

    void ResetVisuals()
    {
        CellCover.Show();
        CellFlag.Hide();
        _.Ground.SelfModulate = Colors.White;
    }
}
