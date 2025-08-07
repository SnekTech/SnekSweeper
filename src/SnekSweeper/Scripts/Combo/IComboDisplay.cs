namespace SnekSweeper.Combo;

public interface IComboDisplay
{
    void DisplayLevelText(string levelText);
    void DisplayProgress(double progressNormalized);
}