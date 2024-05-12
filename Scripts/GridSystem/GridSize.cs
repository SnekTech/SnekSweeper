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

    public GridSize((int row, int column) pair) : this(pair.row, pair.column)
    {
    }

    #region equality stuff

    public bool Equals(GridSize other)
    {
        return Row == other.Row && Column == other.Column;
    }

    public override bool Equals(object obj)
    {
        return obj is GridSize other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Row * 397) ^ Column;
        }
    }

    public static bool operator !=(GridSize a, GridSize b)
    {
        return a.Column != b.Column || a.Row != b.Row;
    }

    public static bool operator ==(GridSize a, GridSize b)
    {
        return !(a != b);
    }

    #endregion
}