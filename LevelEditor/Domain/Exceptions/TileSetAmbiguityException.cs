using System;

namespace LevelEditor.Models
{
    [Serializable]
    public class TileSetAmbiguityException : Exception
    {
        public TileSetAmbiguityException(string message): base(message)
        {

        }
    }
}