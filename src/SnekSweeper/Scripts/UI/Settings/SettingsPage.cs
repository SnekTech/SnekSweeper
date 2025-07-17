using SnekSweeper.Autoloads;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;
using SnekSweeper.SkinSystem;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class SettingsPage : CanvasLayer, ISceneScript
{
    private MainSetting _mainSetting = HouseKeeper.MainSetting;

    public override void _Ready()
    {
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
        var id = SkinOptionButton.GetSelectedId();
        _mainSetting.CurrentSkin = SkinFactory.GetSkinById(id);
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

        var skins = SkinFactory.Skins;
        for (var i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            SkinOptionButton.AddItem(skin.Name, skin.Id);
        }

        var savedSkinIndex = skins.FindIndex(skin => skin.Id == _mainSetting.CurrentSkin.Id);
        if (savedSkinIndex != -1)
        {
            SkinOptionButton.Select(savedSkinIndex);
        }
    }
}