namespace BasicTests.GridTests;

public class MatrixConverterSpecs
{
    [Test]
    public async Task from_matrix_to_list_of_string()
    {
        // todo: data-driven test
        var (matrix, expectedList) = Examples[1];

        var list = MatrixConverter.ToMatrixInList(matrix);

        await Assert.That(list).IsEquivalentTo(expectedList);
    }

    static readonly MatrixListPair[] Examples =
    [
        new(new[,]
            {
                { false, false, false },
                { false, true, false },
                { false, false, false },
            },
            [
                "000",
                "010",
                "000",
            ]),
        
        new(new[,]
            {
                { false, false, false },
                { false, true, false },
                { false, true, true },
            },
            [
                "000",
                "010",
                "011",
            ]),

        new(new bool[,] { }, []),
    ];
}

readonly record struct MatrixListPair(bool[,] Matrix, List<string> MatrixInList);