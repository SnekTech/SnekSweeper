using SnekSweeper.Autoloads;
using SnekSweeper.SaveLoad;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class SkinSelect : HBoxContainer
{
    public override void _Ready()
    {
        InitSkinOptions();
    }

    public override void _EnterTree()
    {
        SkinOptionButton.ItemSelected += OnSkinSelected;
    }
    
    public override void _ExitTree()
    {
        SkinOptionButton.ItemSelected -= OnSkinSelected;
    }
    
    private void OnSkinSelected(long index)
    {
        HouseKeeper.MainSetting.CurrentSkin = SkinFactory.GetSkinById(SkinOptionButton.GetSelectedId()) ?? SkinFactory.Classic;
        SaveLoadEventBus.EmitSaveRequested();
    }
    
    private void InitSkinOptions()
    {
        SkinOptionButton.Clear();

        var skins = SkinFactory.Skins.ToList();
        foreach (var skin in skins)
        {
            SkinOptionButton.AddItem(skin.Name, skin.Id);
        }

        var savedSkinIndex = skins.FindIndex(skin => skin.Id == HouseKeeper.MainSetting.CurrentSkin.Id);
        if (savedSkinIndex != -1)
        {
            SkinOptionButton.Select(savedSkinIndex);
        }
    }
}
