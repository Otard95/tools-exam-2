using System;

namespace LevelEditor.Domain.Exceptions
{
    public class TileMapLogicException : Exception
    {
        public TileMapLogicException(string cannotReplaceNonExistingTileset) : base(cannotReplaceNonExistingTileset)
        {
        }
    }
}