using System.Text.Json;
using System.Text.Json.Serialization;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad;

public class Mat2DConverter : JsonConverter<bool[,]>
{
    static readonly JsonConverter<List<string>> DefaultStringListConverter
        = (JsonConverter<List<string>>)JsonSerializerOptions.Default.GetConverter(typeof(List<string>));

    public override bool[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var mat = DefaultStringListConverter.Read(ref reader, typeof(List<string>), options);
        return mat == null ? null : MatrixExtensions.ToMatrix(mat);
    }

    public override void Write(Utf8JsonWriter writer, bool[,] value, JsonSerializerOptions options)
    {
        var matrixInList = MatrixExtensions.ToMatrixInList(value);
        DefaultStringListConverter.Write(writer, matrixInList, options);
    }
}