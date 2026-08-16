using System.Text.Json;
using System.Text.Json.Serialization;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// Serializes a <c>CellSnapshotState[,]</c> (2D) as nested arrays, mirroring
/// <see cref="Mat2DConverter"/>. Only needed because System.Text.Json does not
/// natively support multidimensional arrays.
/// </summary>
public class CellSnapshotState2DConverter : JsonConverter<CellSnapshotState[,]>
{
    static readonly JsonConverter<int[][]> JaggedArrayConverter =
        (JsonConverter<int[][]>)JsonSerializerOptions.Default.GetConverter(typeof(int[][]));

    public override CellSnapshotState[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jagged = JaggedArrayConverter.Read(ref reader, typeof(int[][]), options)!;
        return MatrixExtensions.FromJagged(jagged).MapTo(x => (CellSnapshotState)x);
    }

    public override void Write(Utf8JsonWriter writer, CellSnapshotState[,] value, JsonSerializerOptions options)
    {
        var matrixInList = value.MapTo(x => (int)x).ToJagged();
        JaggedArrayConverter.Write(writer, matrixInList, options);
    }
}
