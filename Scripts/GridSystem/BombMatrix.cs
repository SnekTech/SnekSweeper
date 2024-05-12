using System;
using Godot;

namespace SnekSweeper.GridSystem;

public class BombMatrix
{
    private readonly bool[,] _bombs;


    public BombMatrix(GridSize size, float probability = 0.3f)
    {
        if (probability is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(probability), "0 ~ 1");
        }

        Size = size;
        var matrix = new bool[size.Row, size.Column];
        foreach (var (i, j) in matrix.Indices())
        {
            matrix[i, j] = GD.Randf() < probability;
        }

        _bombs = matrix;
    }

    public bool this[int i, int j] => _bombs[i, j];
    public GridSize Size { get; }
}