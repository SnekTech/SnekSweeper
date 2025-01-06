using System.Threading.Tasks;
using Godot;
using SnekSweeper.Widgets;

namespace SnekSweeper.CellSystem.Components;

public partial class Flag : Sprite2D, IFlag
{
    public async Task RaiseAsync()
    {
        Show();
        await this.FadeInAsync(0.5f);
    }

    public async Task PutDownAsync()
    {
        await this.FadeOutAsync(0.5f);
        Hide();
    }
}