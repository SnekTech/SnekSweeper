using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SnekSweeper.CheatCode;

[GlobalClass]
public partial class CheatCodeCollection : Resource
{
    [Export] private CheatCodeResource[] cheatCodeResources = null!;

    public List<CheatCodeResource> CheatCodeResources => cheatCodeResources.ToList();
}