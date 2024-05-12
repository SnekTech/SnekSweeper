using Godot;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
	#region constants

	public const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

	#endregion
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetContent(Cell cell)
	{
		var content = GetNode<Content>("Content");
		
		if (cell.HasBomb)
		{
			content.ShowBomb();
		}
		else
		{
			content.ShowNeighbourBombCount(cell.NeighborBombCount);
		}
	}

	public void SetPosition(Cell cell)
	{
		var (i, j) = cell.GridIndex;
		Position = new Vector2(j * CellSizePixels, i * CellSizePixels);
	}
}