using System.Text;

namespace BasicTests.GridTests;

static class MatrixConverter
{
    // public bool[,]? Read()
    // {
    //     var mat = DefaultStringListConverter.Read(ref reader, typeof(List<string>), options);
    //     if (mat == null)
    //         return null;
    //
    //     var rows = mat.Count;
    //     if (rows <= 0)
    //         throw new InvalidOperationException("matrix has <= 0 rows");
    //     var columns = mat[0].Length;
    //     if (columns <= 0)
    //         throw new InvalidOperationException("matrix has <= 0 columns");
    //
    //     var result = new bool[rows, columns];
    //     for (var i = 0; i < rows; i++)
    //     {
    //         for (var j = 0; j < columns; j++)
    //         {
    //             result[i, j] = mat[i][j] != '0';
    //         }
    //     }
    //
    //     return result;
    // }

    public static List<string> ToMatrixInList(bool[,] matrix)
    {
        var rowList = new List<string>();
        var (rows, columns) = (matrix.GetLength(0), matrix.GetLength(1));
        var sb = new StringBuilder();
        for (var i = 0; i < rows; i++)
        {
            sb.Clear();
            for (var j = 0; j < columns; j++)
            {
                sb.Append(matrix[i, j] switch
                {
                    true => '1',
                    false => '0',
                });
            }
            rowList.Add(sb.ToString());
        }

        return rowList;
    }
}