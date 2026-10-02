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

    public void Render(CellOutcome outcome)
    {
        RenderContent(outcome.Info);

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
                Paint(outcome.Info, new CellState.BombRevealed());
                break;
            case CellEvent.FlagTurnedOutWrong:
                Paint(outcome.Info, new CellState.WrongFlagged());
                break;
            default:
                throw new SwitchExpressionException();
        }
    }
    
    public void Paint(CellInfo info, CellState state)
    {
        RenderContent(info);

        switch (state)
        {
            case CellState.Covered:
                CellCover.Show();
                CellFlag.Hide();
                break;
            case CellState.Revealed:
                CellCover.Hide();
                CellFlag.Hide();
                break;
            case CellState.Flagged:
                CellCover.Show();
                CellFlag.Show();
                // todo: decide whether to define a method to paint flag
                CellFlag.FlagSprite.Position = CellFlag.FlagSprite.Position with { Y = 0 };
                break;
            case CellState.BombRevealed:
                CellCover.Hide();
                _.Ground.SelfModulate = Colors.Red;
                break;
            case CellState.WrongFlagged:
                CellCover.Hide();
                CellFlag.Hide();
                Content.MarkAsWrongFlagged();
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
}
