using SnekSweeperCore.GridSystem;

namespace BasicTests.GridTests;

public class MatrixConverterSpecs
{
    [Test]
    [MethodDataSource(typeof(MatrixDataSources), nameof(MatrixDataSources.TestData))]
    public async Task from_matrix_to_list_of_string(MatrixListPair pair)
    {
        var (matrix, expectedList) = pair;

        var list = MatrixExtensions.ToMatrixInList(matrix);

        await Assert.That(list).IsEquivalentTo(expectedList);
    }

    [Test]
    [MethodDataSource(typeof(MatrixDataSources), nameof(MatrixDataSources.TestData))]
    public async Task from_list_of_string_to_matrix(MatrixListPair pair)
    {
        var (expectedMatrix, matrixInList) = pair;

        var list = MatrixExtensions.ToMatrix(matrixInList);

        await Assert.That(list).IsEquivalentTo(expectedMatrix);
    }
}

public readonly record struct MatrixListPair(bool[,] Matrix, List<string> MatrixInList);

public static class MatrixDataSources
{
    public static IEnumerable<Func<MatrixListPair>> TestData()
    {
        yield return () => new MatrixListPair(new[,]
            {
                { false, false, false },
                { false, true, false },
                { false, false, false },
            },
            [
                "000",
                "010",
                "000",
            ]);
        yield return () => new MatrixListPair(new[,]
            {
                { false, false, false },
                { false, true, false },
            },
            [
                "000",
                "010",
            ]);
        yield return () => new MatrixListPair(new[,]
            {
                { false, false, false },
                { false, true, true },
                { false, false, true },
            },
            [
                "000",
                "011",
                "001",
            ]);
        yield return () => new MatrixListPair(new bool[,] { }, []);
        yield return () => new MatrixListPair(new bool[,] { { } }, [""]);
        yield return () => new MatrixListPair(new bool[,] { { }, { } }, ["", ""]);
    }
}