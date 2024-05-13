using Godot;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
	#region constants

	public const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

	#endregion

	private Content Content => GetNode<Content>("Content");
	private Sprite2D Cover => GetNode<Sprite2D>("Cover");
	
	public void SetContent(Cell cell)
	{
		
		if (cell.HasBomb)
		{
			Content.ShowBomb();
		}
		else
		{
			Content.ShowNeighbourBombCount(cell.NeighborBombCount);
		}
	}

	public void SetPosition(Cell cell)
	{
		var (i, j) = cell.GridIndex;
		Position = new Vector2(j * CellSizePixels, i * CellSizePixels);
	}

	public void PutCover()
	{
		Cover.Show();
	}

	public void Reveal()
	{
		Cover.Hide();
	}
}