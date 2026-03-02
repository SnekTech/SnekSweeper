using GodotGadgets.Tasks;
using SnekSweeper.Widgets;
using Widgets.Roguelike;

namespace SnekSweeper;

[SceneTree]
public partial class Main : Node, ISceneScript
{
    public override void _Ready()
    {
        Rand.RandomizeSeed();
    }

    public override void _EnterTree()
    {
        PressToStartLabel.AnyKeyPressed += OnAnyKeyPressed;
    }

    public override void _ExitTree()
    {
        PressToStartLabel.AnyKeyPressed -= OnAnyKeyPressed;
    }

    void OnAnyKeyPressed()
    {
        ShowMainMenuAsync().Fire();
        return;

        async Task ShowMainMenuAsync()
        {
            PressToStartLabel.Hide();
            await MenuContainer.SlideInAsync(MenuContainerTarget.GlobalPosition);
        }
    }
}