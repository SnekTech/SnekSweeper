using Godot;
using SnekSweeper.GridSystem;

namespace SnekSweeper.GameSettings;

[GlobalClass]
public partial class MainSetting : Resource
{
    [Export]
    private GridDifficulty[] _difficulties;

    private GridDifficulty _currentDifficulty;

    public IGridDifficulty CurrentDifficulty => _currentDifficulty ?? _difficulties[0];

    public void SetDifficulty(int index)
    {
        _currentDifficulty = _difficulties[index];
    }
}