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
public partial class ExampleCard : HBoxContainer, ISceneScript, IInitializableContent<ExampleData>
{
    public GridSkin Skin { get; set; } = SkinKey.Classic.ToSkin();

    public void Init(ExampleData exampleData)
    {
        var snapshot = exampleData.Snapshot;

        ExampleDescriptionView.Description = exampleData.Description;

        var grid = new Grid(snapshot.BombMatrix.Size, new CommandInvoker());
        // todo: these two lines should combine?
        TheGrid.InstantiateCells(grid.Size, Skin);
        TheGrid.Init(grid.Size);

        GridParentMarker.Position = GetParentPosition(GridSubViewport.Size, grid.Size.ToPixels());

        ApplyCoverStatus();

        grid.RestoreCellStates(snapshot);
        TheGrid.Paint(grid);

        return;

        // todo: this feels like domain logic, but mostly visual, so now lives in Godot-layer
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
