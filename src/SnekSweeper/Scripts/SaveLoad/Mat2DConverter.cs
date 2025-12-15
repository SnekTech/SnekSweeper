using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnekSweeper.SaveLoad;

public class Mat2DConverter : JsonConverter<bool[,]>
{
    static readonly JsonConverter<int[][]> DefaultJaggedArrayConverter
        = (JsonConverter<int[][]>)PlayerDataJsonExtensions.SerializerOptions.GetConverter(typeof(int[][]));

    public override bool[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var mat = DefaultJaggedArrayConverter.Read(ref reader, typeToConvert, options);

        return mat?.ToMat2D();
    }

    public override void Write(Utf8JsonWriter writer, bool[,] value, JsonSerializerOptions options)
    {
        var jagged = value.ToJagged();
        DefaultJaggedArrayConverter.Write(writer, jagged, options);
    }
}

public class Mat2DConverter2 : JsonConverter<bool[,]>
{
    static readonly JsonConverter<List<string>> DefaultStringListConverter
        = (JsonConverter<List<string>>)PlayerDataJsonExtensions.SerializerOptions.GetConverter(typeof(List<string>));

    public override bool[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var mat = DefaultStringListConverter.Read(ref reader, typeToConvert, options);
        if (mat == null)
            return null;

        var rows = mat.Count;
        if (rows <= 0)
            throw new InvalidOperationException("matrix has <= 0 rows");
        var columns = mat[0].Length;
        if (columns <= 0)
            throw new InvalidOperationException("matrix has <= 0 columns");

        var result = new bool[rows, columns];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                result[i, j] = mat[i][j] != '0';
            }
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, bool[,] value, JsonSerializerOptions options)
    {
        var rowList = new List<string>();
        var (rows, columns) = (value.GetLength(0), value.GetLength(1));
        var sb = new StringBuilder();
        for (var i = 0; i < rows; i++)
        {
            sb.Clear();
            for (var j = 0; j < columns; j++)
            {
                sb.Append(value[i, j] switch
                {
                    true => '1',
                    false => '0',
                });
            }
            rowList.Add(sb.ToString());
        }
        DefaultStringListConverter.Write(writer, rowList, options);
    }
}