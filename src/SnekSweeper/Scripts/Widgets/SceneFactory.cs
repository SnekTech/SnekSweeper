namespace SnekSweeper.Widgets;

public static class SceneFactory
{
    extension<T>(T) where T : Node, ISceneScript
    {
        public static T Instantiate() =>
            ResourceLoader.Load<PackedScene>(T.TscnFilePath).Instantiate<T>();

        /**
         * some init operations may happen during Node._EnterTree hook,
         * which will only be called after adding the node to a parent, not after instantiation
         * so this method make sure to add the node immediately after instantiation, which triggers
         * the _EnterTree method to complete the whole init stage of the newly instantiated scene
         */
        public static T InstantiateOnParent(Node parent)
        {
            var node = SceneFactory.Instantiate<T>();
            parent.AddChild(node);
            return node;
        }
    }
}

public interface ISceneScript
{
    static abstract string TscnFilePath { get; }
}