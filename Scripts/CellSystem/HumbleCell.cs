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

    private Cell _cell;

    private Content Content => GetNode<Content>("Content");
    private Sprite2D Cover => GetNode<Sprite2D>("Cover");
    private Area2D ClickArea => GetNode<Area2D>("ClickArea");

    public override void _Ready()
    {
        ClickArea.InputEvent += OnPrimary;
    }

    public override void _ExitTree()
    {
        ClickArea.InputEvent -= OnPrimary;
    }

    private void OnPrimary(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event.IsActionReleased("primary"))
        {
            GD.Print(_cell.GridIndex);
        }
    }

    public void Init(Cell cell)
    {
        _cell = cell;
        SetContent();
        SetPosition();
    }

    private void SetContent()
    {
        if (_cell.HasBomb)
        {
            Content.ShowBomb();
        }
        else
        {
            Content.ShowNeighbourBombCount(_cell.NeighborBombCount);
        }
    }

    private void SetPosition()
    {
        var (i, j) = _cell.GridIndex;
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