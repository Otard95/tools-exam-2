using System;

namespace LevelEditor.Services
{
    public class TileMapLogicException : Exception
    {
        public TileMapLogicException(string cannotReplaceNonExistingTileset) : base(cannotReplaceNonExistingTileset)
        {
        }
    }
}