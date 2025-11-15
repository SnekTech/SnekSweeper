namespace SnekSweeper.NativeTools;

public static class LayMineEngine
{
    public static bool[,] LayMineSolvable(int rows, int columns, int mineNum, int x0, int y0, int maxTimes = 100_0000)
    {
        var bombMatrix = new bool[rows, columns];
        unsafe
        {
            var buffer = NativeMethods.lay_mine_solvable_cs((nuint)rows, (nuint)columns, (nuint)mineNum,
                (nuint)x0, (nuint)y0, (nuint)maxTimes);
            var bombs1d = buffer->AsSpan<int>().ToArray();
            NativeMethods.free_i32_buffer(buffer);

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