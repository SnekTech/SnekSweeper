using Godot;
using SnekSweeper.GridSystem;

namespace SnekSweeper.GameSettings;

[GlobalClass]
public partial class MainSetting : Resource
{
    [Export]
    private GridDifficulty[] _difficulties = null!;

    private GridDifficulty? _currentDifficulty;

    public IGridDifficulty CurrentDifficulty => _currentDifficulty ?? _difficulties[1];

    public void SetDifficulty(int index = 0)
    {
        _currentDifficulty = _difficulties[index];
    }
}