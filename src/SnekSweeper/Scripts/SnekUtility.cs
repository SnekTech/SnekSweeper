namespace SnekSweeper;

public static class SnekUtility
{
    public static Texture2D LoadTexture(string path) => ResourceLoader.Load<Texture2D>(path);
}