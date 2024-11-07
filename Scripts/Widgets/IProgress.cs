namespace SnekSweeper.Widgets;

public interface IProgress
{
    float Value { get; set; }
    float MaxValue { get; set; }
    
    // maybe style setter
}