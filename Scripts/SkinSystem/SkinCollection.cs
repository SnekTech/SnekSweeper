using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SnekSweeper.SkinSystem;

[GlobalClass]
public partial class SkinCollection : Resource
{
    [Export]
    private SkinResource[] _skins = null!;

    private Dictionary<string, ISkin> SkinDict
    {
        get
        {
            return _skins.ToDictionary<SkinResource, string, ISkin>(skin => skin.Name, skin => skin);
        }
    }
    
    public ISkin GetSkin(string skinName)
    {
        return SkinDict[skinName];
    }
}