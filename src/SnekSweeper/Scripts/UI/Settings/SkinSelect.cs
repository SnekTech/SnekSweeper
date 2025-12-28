using SnekSweeper.Autoloads;
using SnekSweeper.SkinSystem;
using SnekSweeperCore.SkinSystem;

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
    
    static void OnSkinSelected(long index)
    {
        HouseKeeper.MainSetting.CurrentSkinKey = SkinKey.FromLong(index);
        HouseKeeper.SaveCurrentPlayerData();
    }
    
    void InitSkinOptions()
    {
        SkinOptionButton.Clear();

        var skins = SkinFactory.Skins.ToList();
        foreach (var skin in skins)
        {
            SkinOptionButton.AddItem(skin.Name, (int)skin.Key);
        }

        var savedSkinIndex = skins.FindIndex(skin => skin.Key == HouseKeeper.MainSetting.CurrentSkinKey);
        if (savedSkinIndex != -1)
        {
            SkinOptionButton.Select(savedSkinIndex);
        }
    }
}
