namespace SnekSweeper.Widgets;

public static class SceneFactory
{
    public static T Instantiate<T>() where T : Node, ISceneScript =>
        ResourceLoader.Load<PackedScene>(T.TscnFilePath).Instantiate<T>();
}

public interface ISceneScript
{
    static abstract string TscnFilePath { get; }
}