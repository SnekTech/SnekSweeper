using Dumpify;
using SnekSweeper.Autoloads;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class SettingsPage : CanvasLayer, ISceneScript
{
    [Export]
    private SkinCollection skinCollection = null!;

    private MainSetting _mainSetting = null!;

    public override void _Ready()
    {
        _mainSetting = HouseKeeper.MainSetting;
        GD.Print(_mainSetting.DumpText());

        GenerateDifficultyOptions();
        GenerateSkinOptions();
        DifficultyOptionButton.ItemSelected += OnDifficultySelected;
        SkinOptionButton.ItemSelected += OnSkinSelected;
    }

    public override void _ExitTree()
    {
        DifficultyOptionButton.ItemSelected -= OnDifficultySelected;
        SkinOptionButton.ItemSelected -= OnSkinSelected;
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
        DifficultyOptionButton.Clear();
        for (var i = 0; i < _mainSetting.Difficulties.Length; i++)
        {
            var difficulty = _mainSetting.Difficulties[i];
            DifficultyOptionButton.AddItem(difficulty.Name, i);
        }

        DifficultyOptionButton.Select(_mainSetting.CurrentDifficultyIndex);
    }

    private void GenerateSkinOptions()
    {
        SkinOptionButton.Clear();
        var skins = skinCollection.Skins;
        for (var i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            SkinOptionButton.AddItem(skin.Name, i);
        }

        SkinOptionButton.Select(_mainSetting.CurrentSkinIndex);
    }
}