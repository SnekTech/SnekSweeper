using GodotGadgets.UI.Pagination;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial.Example;

[SceneTree]
public partial class ExampleCard : HBoxContainer, ISceneScript, IInitializableContent<ExampleData>
{
    public GridSkin Skin { get; set; } = SkinKey.Classic.ToSkin();

    public void Init(ExampleData exampleData)
    {
        var snapshot = exampleData.Snapshot;

        ExampleDescriptionView.Description = exampleData.Description;

        var grid = new Grid(snapshot.BombMatrix.Size);
        TheGrid.Init(grid.Size, Skin);

        GridParentMarker.Position = GetParentPosition(GridSubViewport.Size, grid.Size.ToPixels());

        ApplyCoverStatus();

        grid.RestoreCellStates(snapshot);
        TheGrid.Paint(grid);

        return;

        void ApplyCoverStatus()
        {
            foreach (var safeIndex in exampleData.SafeCoveredCells)
            {
                TheGrid.HumbleCellsContainer.CellAt(safeIndex).Cover.SetStatus(CoverStatus.Safe);
            }

            foreach (var uncertainIndex in exampleData.UncertainCoveredCells)
            {
                TheGrid.HumbleCellsContainer.CellAt(uncertainIndex).Cover.SetStatus(CoverStatus.Uncertain);
            }
        }
    }

    static Vector2 GetParentPosition(Vector2 containerSize, Vector2 targetSize)
    {
        var center = containerSize / 2;
        return center - targetSize / 2;
    }
}