using System.Text;

namespace BasicTests.GridTests;

static class MatrixConverter
{
    public static bool[,] ToMatrix(List<string> matrixInList)
    {
        var rows = matrixInList.Count;

        var columns = rows > 0 ? matrixInList[0].Length : 0;
    
        var result = new bool[rows, columns];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                result[i, j] = matrixInList[i][j] != '0';
            }
        }
    
        return result;
    }

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