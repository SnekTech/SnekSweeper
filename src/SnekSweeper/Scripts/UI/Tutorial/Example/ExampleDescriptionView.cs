using GodotGadgets.Extensions;
using SnekSweeper.Widgets;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial.Example;

[SceneTree]
public partial class ExampleDescriptionView : VBoxContainer
{
    public ExampleDescription Description
    {
        set => UpdateContent(value);
    }

    void UpdateContent(ExampleDescription newDescription)
    {
        Title.Text = newDescription.Title;
        
        DescriptionSentenceContainer.ClearChildren();
        foreach (var sentenceText in newDescription.Sentences)
        {
            var sentence = ExampleSentence.InstantiateOnParent(DescriptionSentenceContainer);
            sentence.Text = sentenceText;
        }
    }
}
