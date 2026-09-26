using System.Threading.Tasks;
using GodotGadgets.UI.Pagination;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial.Example;

[SceneTree]
public partial class ExampleCard : HBoxContainer, ISceneScript, IAsyncContent<ExampleData>
{
    public GridSkin Skin { get; set; } = SkinKey.Classic.ToSkin();

    public async Task InitAsync(ExampleData exampleData, CancellationToken ct = default)
    {
        var snapshot = exampleData.Snapshot;

        ExampleDescriptionView.Description = exampleData.Description;

        var grid = Grid.Create(TheGrid.CellFactory, snapshot.BombMatrix.Size, Skin, new GridEventBus(), new CommandInvoker());
        TheGrid.Init(grid.Size);
        
        ct.ThrowIfCancellationRequested();

        GridParentMarker.Position = GetParentPosition(GridSubViewport.Size, grid.Size.ToPixels());

        // 封面状态放在 await **之前**：HumbleCell（含 Cover）在 Grid.Create 时就已同步建好，不必等 InitCellsAsync。
        // 放到 await 之后，会让"卡片已被换页清掉、续体才回来"的那一帧撞上死节点（审计里的 #4）；
        // 搬前之后，本方法在 await 之后零节点访问。
        ApplyCoverStatus();

        await grid.InitCellsAsync(snapshot, ct);

        return;

        void ApplyCoverStatus()
        {
            foreach (var safeIndex in exampleData.SafeCoveredCells)
            {
                grid.GetCellAt(safeIndex).HumbleCell.Cover.SetStatus(CoverStatus.Safe);
            }

            foreach (var uncertainIndex in exampleData.UncertainCoveredCells)
            {
                grid.GetCellAt(uncertainIndex).HumbleCell.Cover.SetStatus(CoverStatus.Uncertain);
            }
        }
    }

    static Vector2 GetParentPosition(Vector2 containerSize, Vector2 targetSize)
    {
        var center = containerSize / 2;
        return center - targetSize / 2;
    }
}
