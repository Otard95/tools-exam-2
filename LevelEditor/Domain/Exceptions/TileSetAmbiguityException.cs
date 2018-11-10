using System;

namespace LevelEditor.Models
{
    public class TileSetAmbiguityException : Exception
    {
        public TileSetAmbiguityException(string message): base(message)
        {

        }
    }
}