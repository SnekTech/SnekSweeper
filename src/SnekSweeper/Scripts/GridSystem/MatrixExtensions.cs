namespace SnekSweeper.GridSystem;

public static class MatrixExtensions
{
    public static GridSize Size<T>(this T[,] matrix)
    {
        return new GridSize(matrix.GetLength(0), matrix.GetLength(1));
    }

    public static IEnumerable<GridIndex> Indices<T>(this T[,] matrix)
    {
        var (rows, columns) = matrix.Size();
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                yield return new GridIndex(i, j);
            }
        }
    }
}