namespace SnekSweeper.Combo;

public class BasicComboDisplay(Label levelLabel, ProgressBar progressBar): IComboDisplay
{
    public void DisplayLevelText(string levelText)
    {
        levelLabel.Text = levelText;
    }

    public void DisplayProgress(double progressNormalized)
    {
        progressBar.Value = progressNormalized * progressBar.MaxValue;
    }
}