using System.Collections.Generic;

namespace SnekSweeper.GridSystem;

public static class MatrixExtensions
{
    public static (int rows, int columns) Size<T>(this T[,] matrix)
    {
        return (matrix.GetLength(0), matrix.GetLength(1));
    }

    public static IEnumerable<(int i, int j)> Indices<T>(this T[,] matrix)
    {
        var (rows, columns) = matrix.Size();
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                yield return (i, j);
            }
        }
    }

    public static IEnumerable<T> Values<T>(this T[,] matrix)
    {
        if (matrix.Length == 0)
            yield break;

        var enumerator = matrix.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return ((T)enumerator.Current)!;
        }
    }
}