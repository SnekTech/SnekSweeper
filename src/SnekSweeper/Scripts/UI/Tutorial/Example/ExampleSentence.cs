using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Tutorial.Example;

[SceneTree]
public partial class ExampleSentence : HBoxContainer, ISceneScript
{
    public string Text
    {
        set => _.Sentence.Text = value;
    }
}