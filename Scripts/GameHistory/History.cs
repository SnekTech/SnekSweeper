using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.GameHistory;

public partial class History : Resource
{
    [Export]
    private Array<Record> _records = new();
    
    public void AddRecord(Record record)
    {
        _records.Add(record);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public void ClearRecords()
    {
        _records.Clear();
    }
    
    public List<Record> Records => _records.ToList();
}