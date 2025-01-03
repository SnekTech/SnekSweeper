using System;
using Widgets.Roguelike;

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

        var (rows, columns) = gridDifficulty.Size;
        var matrix = new bool[rows, columns];
        foreach (var (i, j) in matrix.Indices())
        {
            matrix[i, j] = Rand.Float() < probability;
        }

        _bombs = matrix;
    }

    public (int rows, int columns) Size => _bombs.Size();

    public bool this[int i, int j] => _bombs[i, j];

    public void ClearBombAt(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        _bombs[i, j] = false;
    }
}