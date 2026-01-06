namespace SnekSweeperCore.GridSystem;

public static class MatrixExtensions
{
    extension<T>(T[,] matrix)
    {
        public GridSize Size => new(matrix.GetLength(0), matrix.GetLength(1));

        public IEnumerable<GridIndex> Indices()
        {
            var (rows, columns) = matrix.Size;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    yield return new GridIndex(i, j);
                }
            }
        }

        public IEnumerable<T> Elements => matrix.Indices().Select(matrix.At);

        public T At(GridIndex index) => matrix[index.I, index.J];
        public void SetAt(GridIndex index, T value) => matrix[index.I, index.J] = value;

        public TU[,] MapTo<TU>(Func<T, TU> mapper) => matrix.MapTo((element, _) => mapper(element));

        public TU[,] MapTo<TU>(Func<T, GridIndex, TU> mapper) =>
            Create<TU>(matrix.Size, gridIndex => mapper(matrix.At(gridIndex), gridIndex));

        public static T[,] Create(GridSize size, Func<GridIndex, T> createFn)
        {
            var mat = new T[size.Rows, size.Columns];
            foreach (var gridIndex in mat.Indices())
            {
                var value = createFn(gridIndex);
                mat.SetAt(gridIndex, value);
            }

            return mat;
        }
    }
}