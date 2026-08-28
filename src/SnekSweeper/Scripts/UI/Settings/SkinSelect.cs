using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.SkinSystem;
using SnekSweeperCore.SaveLoad;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.UI.Settings;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class SkinSelect : HBoxContainer
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _EnterTree()
    {
        SkinOptionButton.ItemSelected += OnSkinSelected;
    }
    
    public override void _ExitTree()
    {
        SkinOptionButton.ItemSelected -= OnSkinSelected;
    }
    
    void OnSkinSelected(long index)
    {
        SaveData.UpdateMainSetting(m => m with { CurrentSkinKey = SkinKey.FromLong(index) });
    }
    
    public void OnResolved()
    {
        SkinOptionButton.Clear();

        var skins = SkinFactory.Skins.ToList();
        foreach (var skin in skins)
        {
            SkinOptionButton.AddItem(skin.Name, (int)skin.Key);
        }

        var savedSkinIndex = skins.FindIndex(skin => skin.Key == SaveData.State.MainSetting.CurrentSkinKey);
        if (savedSkinIndex != -1)
        {
            SkinOptionButton.Select(savedSkinIndex);
        }
    }
}
