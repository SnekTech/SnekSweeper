using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.CellSystem.StateMachine;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.CellSystem;

[SceneTree]
public partial class HumbleCell : Node2D, IHumbleCell, ISceneScript
{
    public const int CellSizeInPixels = 16;

    // 纯逻辑状态机用属性初始化器创建：Core 在 InstantiateCell 返回后立刻读取 Logic，
    // 此时 _Ready 未必已触发，所以不能像场景节点那样放 _Ready 里（那会是 null!）。
    public CellLogic Logic { get; } = new();

    // 绑定要访问场景节点（Cover/Flag），放 _Ready 建立；_ExitTree 释放。
    LogicBlock.Binding _binding = null!;

    public override void _Ready()
    {
        _binding = Logic.Bind()
            .OnOutput((in CellState.Output.CoverRevealed output) => Cover.RevealAsync().AsGDTask().Forget())
            .OnOutput((in CellState.Output.CoverPutOn output) => Cover.PutOnAsync().AsGDTask().Forget())
            .OnOutput((in CellState.Output.FlagRaised output) => Flag.RaiseAsync().AsGDTask().Forget())
            .OnOutput((in CellState.Output.FlagPutDown output) => Flag.PutDownAsync().AsGDTask().Forget())
            .OnOutput((in CellState.Output.MarkedAsBombRevealed output) => MarkAsBombRevealed())
            .OnOutput((in CellState.Output.MarkedAsWrongFlagged output) => MarkAsWrongFlagged());
    }

    public override void _ExitTree()
    {
        _binding?.Dispose();
    }

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
