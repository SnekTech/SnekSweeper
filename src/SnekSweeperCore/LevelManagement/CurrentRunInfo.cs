namespace SnekSweeperCore.LevelManagement;

public class CurrentRunInfo
{
    public GridSystem.GridSnapshot? GridSnapshot { get; set; }
    
    public RunStartInfo StartInfo { get; set; }
}