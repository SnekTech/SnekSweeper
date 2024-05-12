using System;

namespace SnekSweeper.GridSystem;

public struct GridIndex
{
    public readonly int Row;
    public readonly int Column;

    public GridIndex(int row, int column)
    {
        (Row, Column) = (row, column);
    }

    public static GridIndex First => new(0, 0);

    public static GridIndex operator +(GridIndex a, GridIndex b)
    {
        return new GridIndex(a.Row + b.Row, a.Column + b.Column);
    }

    #region equality stuff

    public bool Equals(GridIndex other)
    {
        return (Row, Column) == (other.Row, other.Column);
    }

    public override bool Equals(object obj)
    {
        return obj is GridIndex other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    public static bool operator !=(GridIndex a, GridIndex b)
    {
        return (a.Row, a.Column) != (b.Row, b.Column);
    }

    public static bool operator ==(GridIndex a, GridIndex b)
    {
        return !(a != b);
    }

    #endregion
}