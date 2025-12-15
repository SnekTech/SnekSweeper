namespace SnekSweeper.GridSystem;

public static class MatrixExtensions
{
    extension<T>(T[,] matrix)
    {
        GridSize Size() => new(matrix.GetLength(0), matrix.GetLength(1));

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

        internal IEnumerable<T> Elements => matrix.Indices().Select(matrix.At);

        internal TU[,] MapTo<TU>(Func<T, TU> f)
        {
            var size = matrix.Size();
            var mappedMatrix = new TU[size.Rows, size.Columns];
            foreach (var gridIndex in matrix.Indices())
            {
                mappedMatrix.SetAt(gridIndex, f(matrix.At(gridIndex)));
            }
            return mappedMatrix;
        }

        public T At(GridIndex index) => matrix[index.I, index.J];
        public void SetAt(GridIndex index, T value) => matrix[index.I, index.J] = value;
    }
}