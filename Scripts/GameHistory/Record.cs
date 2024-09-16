using System;
using Godot;

namespace SnekSweeper.GameHistory;

public partial class Record : Resource
{
    /// <summary>
    /// use long to save and load <see cref="DateTime"/> binary
    /// </summary>
    [Export]
    private long _startDateTime;

    [Export]
    private long _endDateTime;

    [Export]
    private bool _winning;

    public Record()
    {
    }

    public Record(DateTime startAt, DateTime endAt, bool winning)
    {
        _startDateTime = startAt.ToBinary();
        _endDateTime = endAt.ToBinary();
        _winning = winning;
    }

    public DateTime StartAt => DateTime.FromBinary(_startDateTime);

    public DateTime EndAt => DateTime.FromBinary(_endDateTime);

    public bool Winning => _winning;
}