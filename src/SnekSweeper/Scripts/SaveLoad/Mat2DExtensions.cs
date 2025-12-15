namespace SnekSweeper.SaveLoad;

static class Mat2DExtensions
{
    extension(bool[,] bombs)
    {
        public int[][] ToJagged()
        {
            if (bombs.Rank != 2)
                throw new InvalidOperationException("the bomb matrix is not 2d");

            var (rows, columns) = (bombs.GetLength(0), bombs.GetLength(1));
            if (rows <= 0 || columns <= 0)
                throw new InvalidOperationException("the bomb matrix has <0 rows or columns");

            var result = new int[rows][];
            for (var i = 0; i < rows; i++)
            {
                result[i] = new int[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = bombs[i, j] switch
                    {
                        true => 1,
                        false => 0,
                    };
                }
            }

            return result;
        }
    }

    extension(int[][] jagged)
    {
        public bool[,] ToMat2D()
        {
            if (jagged.Length <= 0)
                throw new InvalidOperationException("the jagged array has <0 rows");
            if (jagged[0].Length <= 0)
                throw new InvalidOperationException("the jagged array has <0 columns");

            var (rows, columns) = (jagged.Length, jagged[0].Length);
            var result = new bool[rows, columns];
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result[i, j] = jagged[i][j] != 0;
                }
            }

            return result;
        }
    }
}