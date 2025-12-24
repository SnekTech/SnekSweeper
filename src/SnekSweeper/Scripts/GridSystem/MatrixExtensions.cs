using SnekSweeperCore.GridSystem.Difficulty;

namespace SnekSweeper.GridSystem;

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

        internal IEnumerable<T> Elements => matrix.Indices().Select(matrix.At);

        public T At(GridIndex index) => matrix[index.I, index.J];
        public void SetAt(GridIndex index, T value) => matrix[index.I, index.J] = value;

        public TU[,] MapTo<TU>(Func<T, TU> f)
        {
            return matrix.MapTo((cell, _) => f(cell));
        }

        public TU[,] MapTo<TU>(Func<T, GridIndex, TU> f)
        {
            var (rows, columns) = matrix.Size;
            var mapped = new TU[rows, columns];
    
            foreach (var gridIndex in matrix.Indices())
            {
                var mappedElement = f(matrix.At(gridIndex), gridIndex);
                mapped.SetAt(gridIndex, mappedElement);
            }
            
            return mapped;
            
        }
    }
}