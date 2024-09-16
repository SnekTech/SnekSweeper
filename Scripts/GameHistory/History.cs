using Godot;
using Godot.Collections;

namespace SnekSweeper.GameHistory;

public partial class History : Resource
{
    [Export]
    private Array<Record> _records = new();

    public void AddRecord(Record record)
    {
        _records.Add(record);
    }

    public void ClearRecords()
    {
        _records.Clear();
    }
}