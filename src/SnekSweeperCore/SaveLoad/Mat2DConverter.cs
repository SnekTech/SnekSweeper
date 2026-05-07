using System.Text.Json;
using System.Text.Json.Serialization;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad;

public class Mat2DConverter : JsonConverter<bool[,]>
{
    static readonly JsonConverter<int[][]> JaggedArrayConverter =
        (JsonConverter<int[][]>)JsonSerializerOptions.Default.GetConverter(typeof(int[][]));

    public override bool[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jagged = JaggedArrayConverter.Read(ref reader, typeof(int[][]), options)!;
        return MatrixExtensions.FromJagged(jagged).MapTo(x => x != 0);
    }

    public override void Write(Utf8JsonWriter writer, bool[,] value, JsonSerializerOptions options)
    {
        var matrixInList = value.MapTo(x => x ? 1 : 0).ToJagged();
        JaggedArrayConverter.Write(writer, matrixInList, options);
    }
}