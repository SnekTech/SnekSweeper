using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common.AmbientBackground;

/// <summary>
/// 背景主题沙盒: 每个主题一个按钮, 点击即过渡过去(用于预览观感 / 调参)。
/// </summary>
[SceneTree]
public partial class AmbientBackgroundDemo : Control, ISceneScript
{
    const float TransitionDuration = 0.8f;

    // 新增主题时在这里加一行
    static readonly (string Label, AmbientTheme Theme)[] Themes =
    [
        ("Menu", AmbientThemes.Menu),
        ("Level", AmbientThemes.Level),
    ];

    public override void _Ready()
    {
        foreach (var (label, theme) in Themes)
        {
            var button = new Button { Text = label };
            button.Pressed += () => _.Background.GoTo(theme, TransitionDuration);
            _.Buttons.AddChild(button);
        }
    }
}
