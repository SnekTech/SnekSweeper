namespace SnekSweeper.GridSystem;

public static class MatrixExtensions
{
    extension<T>(T[,] matrix)
    {
        private GridSize Size() => new(matrix.GetLength(0), matrix.GetLength(1));

        public IEnumerable<GridIndex> Indices()
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

        public T At(GridIndex index) => matrix[index.I, index.J];
        public void SetAt(GridIndex index, T value) => matrix[index.I, index.J] = value;
    }
}