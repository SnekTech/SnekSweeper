namespace SnekSweeper.Widgets;

public static class SceneFactory
{
    extension<T>(T) where T : Node, ISceneScript
    {
        public static T Instantiate() =>
            ResourceLoader.Load<PackedScene>(T.TscnFilePath).Instantiate<T>();
    }
}

public interface ISceneScript
{
    static abstract string TscnFilePath { get; }
}