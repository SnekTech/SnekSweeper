using System.Text.Json.Serialization;
using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GameSettings;

[GlobalClass]
public partial class MainSetting : Resource
{
    public readonly IGridDifficulty[] Difficulties =
    [
        new GridDifficulty("simple", (5, 5), 0.1f),
        new GridDifficulty("medium", (10, 10), 0.1f),
        new GridDifficulty("hard", (10, 10), 0.2f),
    ];

    [Export]
    private int _currentDifficultyIndex = 1;

    public int CurrentDifficultyIndex
    {
        get => _currentDifficultyIndex;
        set
        {
            _currentDifficultyIndex = value;

            if (value != _currentDifficultyIndex)
                EmitChanged();
        }
    }

    public IGridDifficulty CurrentDifficulty => Difficulties[_currentDifficultyIndex];

    public string CurrentSkinName { get; set; } = string.Empty;
    
    [JsonIgnore]
    public ISkin CurrentSkin => SkinFactory.GetSkinByName(CurrentSkinName);
}