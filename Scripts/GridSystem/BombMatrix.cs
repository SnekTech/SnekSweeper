using System;
using Godot;

namespace SnekSweeper.GridSystem;

public class BombMatrix
{
    private readonly bool[,] _bombs;

    public BombMatrix(IGridDifficulty gridDifficulty)
    {
        var probability = gridDifficulty.BombPercent;
        if (probability is < 0 or > 1)
        {
            throw new ArgumentException("0 ~ 1", nameof(gridDifficulty));
        }

        Size = gridDifficulty.Size;
        var matrix = new bool[Size.rows, Size.columns];
        foreach (var (i, j) in matrix.Indices())
        {
            matrix[i, j] = GD.Randf() < probability;
        }

        _bombs = matrix;
    }

    public bool this[int i, int j] => _bombs[i, j];
    public (int rows, int columns) Size { get; }
}