namespace SnekSweeper.NativeTools;

public static class LayMineEngine
{
    public static bool[,] LayMineSolvable(int rows, int columns, int mineNum, int x0, int y0, int maxTimes)
    {
        var bombMatrix = new bool[rows, columns];
        unsafe
        {
            var buffer = NativeMethods.lay_mine_solvable_cs((nuint)rows, (UIntPtr)columns, (UIntPtr)mineNum,
                (UIntPtr)x0, (UIntPtr)y0, (UIntPtr)maxTimes);
            var bombs1d = buffer->AsSpan<int>().ToArray();
            for (var i = 0u; i < rows; i++)
            {
                for (var j = 0u; j < columns; j++)
                {
                    bombMatrix[i, j] = bombs1d[i * columns + j] == -1;
                }
            }
        }

        return bombMatrix;
    }
}