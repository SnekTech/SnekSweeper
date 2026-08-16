using System.Text.Json;
using System.Text.Json.Serialization;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad.CustomJsonConverter;

/// <summary>
/// Serializes an integer-backed <c>T[,]</c> matrix as nested arrays.
/// Only needed because System.Text.Json does not natively support
/// multidimensional arrays. Subclasses supply the element conversions
/// (e.g. <see cref="bool"/> or an enum such as <see cref="CellSnapshotState"/>).
/// </summary>
public abstract class Matrix2DConverter<T> : JsonConverter<T[,]>
    where T : struct
{
    protected abstract int ElementToInt(T value);
    protected abstract T IntToElement(int value);

    public override bool HandleNull => false;

    public override T[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("A 2D matrix cannot be null.");
        }

        var jagged = JaggedIntArrayConverterHolder.Instance.Read(ref reader, typeof(int[][]), options)!;
        return MatrixExtensions.FromJagged(jagged).MapTo(IntToElement);
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "A 2D matrix cannot be null.");
        }

        var matrixInList = value.MapTo(ElementToInt).ToJagged();
        JaggedIntArrayConverterHolder.Instance.Write(writer, matrixInList, options);
    }
}

/// <summary>
/// Caches the <c>int[][]</c> converter used as the on-disk representation by
/// <see cref="Matrix2DConverter{T}"/> subclasses. It lives outside the generic
/// type so every closed type shares a single instance (avoids CA1000).
/// </summary>
static class JaggedIntArrayConverterHolder
{
    public static readonly JsonConverter<int[][]> Instance =
        (JsonConverter<int[][]>)JsonSerializerOptions.Default.GetConverter(typeof(int[][]));
}
