using System;
using Godot;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public partial class HumbleCell : Node2D, IHumbleCell
{
    #region constants

    public const int CellSizePixels = 16;
    public const string CellScenePath = "res://Scenes/cell.tscn";

    #endregion
    
    public event Action PrimaryReleased;

    private Content Content => GetNode<Content>("Content");
    private Sprite2D Cover => GetNode<Sprite2D>("Cover");
    private Area2D ClickArea => GetNode<Area2D>("ClickArea");

    public override void _Ready()
    {
        ClickArea.InputEvent += OnPrimaryReleased;
    }

    public override void _ExitTree()
    {
        ClickArea.InputEvent -= OnPrimaryReleased;
    }

    private void OnPrimaryReleased(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event.IsActionReleased("primary"))
        {
            PrimaryReleased?.Invoke();
        }
    }

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

    public void SetPosition((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
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