using Godot;

namespace SnekSweeper.GridSystem;

[GlobalClass]
public partial class GridDifficulty : Resource, IGridDifficulty
{
    [Export(PropertyHint.Range, "5,20,")]
    private int _rows = 10;

    [Export(PropertyHint.Range, "5,30,")]
    private int _columns = 10;

    [Export(PropertyHint.Range, "0,100,10")]
    private int _bombPercentInt = 10;

    public (int rows, int columns) Size => (_rows, _columns);
    public float BombPercent => _bombPercentInt / 100f;
}