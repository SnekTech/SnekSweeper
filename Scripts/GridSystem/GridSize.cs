using System;

namespace SnekSweeper.GridSystem;

public struct GridSize
{
    public int Row;
    public int Column;

    public int Total => Row * Column;

    public GridSize(int row, int column)
    {
        (Row, Column) = (row, column);
    }

    #region equality stuff

    public bool Equals(GridSize other)
    {
        return (Row, Column) == (other.Row, other.Column);
    }

    public override bool Equals(object obj)
    {
        return obj is GridSize other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    public static bool operator !=(GridSize a, GridSize b)
    {
        return (a.Row, a.Column) != (b.Row, b.Column);
    }

    public static bool operator ==(GridSize a, GridSize b)
    {
        return !(a != b);
    }

    #endregion
}