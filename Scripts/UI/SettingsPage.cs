using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.UI;

[Scene]
public partial class SettingsPage : CanvasLayer
{
    [Export] private SkinCollection skinCollection = default!;

    [Node] private OptionButton difficultyOptionButton = default!;
    [Node] private OptionButton skinOptionButton = default!;

    private MainSetting _mainSetting = default!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        _mainSetting = HouseKeeper.MainSetting;

        GenerateDifficultyOptions();
        GenerateSkinOptions();
        difficultyOptionButton.ItemSelected += OnDifficultySelected;
        skinOptionButton.ItemSelected += OnSkinSelected;
    }

    public override void _ExitTree()
    {
        difficultyOptionButton.ItemSelected -= OnDifficultySelected;
        skinOptionButton.ItemSelected -= OnSkinSelected;
    }

    private void OnDifficultySelected(long index)
    {
        _mainSetting.CurrentDifficultyIndex = (int)index;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void OnSkinSelected(long index)
    {
        _mainSetting.CurrentSkinIndex = (int)index;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void GenerateDifficultyOptions()
    {
        difficultyOptionButton.Clear();
        for (var i = 0; i < _mainSetting.Difficulties.Length; i++)
        {
            var difficulty = _mainSetting.Difficulties[i];
            difficultyOptionButton.AddItem(difficulty.Name, i);
        }

        difficultyOptionButton.Select(_mainSetting.CurrentDifficultyIndex);
    }

    private void GenerateSkinOptions()
    {
        skinOptionButton.Clear();
        var skins = skinCollection.Skins;
        for (var i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            skinOptionButton.AddItem(skin.Name, i);
        }

        skinOptionButton.Select(_mainSetting.CurrentSkinIndex);
    }
}