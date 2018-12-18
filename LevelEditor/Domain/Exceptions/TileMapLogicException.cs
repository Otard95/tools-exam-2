using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    public class TileMapLogicException : Exception
    {
        public TileMapLogicException(string cannotReplaceNonExistingTileset) : base(cannotReplaceNonExistingTileset)
        {
        }
    }
}