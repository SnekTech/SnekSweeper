using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad.CustomJsonConverter;

/// <summary>
/// Serializes a <c>CellSnapshotState[,]</c> matrix as nested arrays.
/// </summary>
public sealed class CellSnapshotState2DConverter : Matrix2DConverter<CellSnapshotState>
{
    protected override int ElementToInt(CellSnapshotState value) => (int)value;
    protected override CellSnapshotState IntToElement(int value) => (CellSnapshotState)value;
}
