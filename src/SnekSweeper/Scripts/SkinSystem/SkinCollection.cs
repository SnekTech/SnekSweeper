using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SnekSweeper.SkinSystem;

[GlobalClass]
public partial class SkinCollection : Resource
{
    [Export]
    private SkinResource[] _skins = null!;

    public List<ISkin> Skins => _skins.Cast<ISkin>().ToList();
}