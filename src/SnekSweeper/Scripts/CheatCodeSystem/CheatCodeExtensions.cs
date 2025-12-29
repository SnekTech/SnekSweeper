using SnekSweeperCore.CheatCodeSystem;

namespace SnekSweeper.CheatCodeSystem;

static class CheatCodeExtension
{
    extension(CheatCode cheatCode)
    {
        internal Texture2D Icon => SnekUtility.LoadTexture(cheatCode.Data.IconPath);
    }
}