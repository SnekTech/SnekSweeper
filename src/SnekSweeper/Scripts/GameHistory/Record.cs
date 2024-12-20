using System;
using Godot;

namespace SnekSweeper.GameHistory;

public partial class Record : Resource
{
    /// <summary>
    /// use long to save and load <see cref="DateTime"/> binary
    /// </summary>
    [Export]
    private long _startDateTimeBinary;

    [Export]
    private long _endDateTimeBinary;

    [Export]
    private bool _winning;

    public Record()
    {
    }

    public Record(DateTime startAt, DateTime endAt, bool winning)
    {
        _startDateTimeBinary = startAt.ToBinary();
        _endDateTimeBinary = endAt.ToBinary();
        _winning = winning;
    }

    public DateTime StartAt => DateTime.FromBinary(_startDateTimeBinary);

    public DateTime EndAt => DateTime.FromBinary(_endDateTimeBinary);

    public bool Winning => _winning;
}