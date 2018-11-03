using System;

namespace LevelEditor.Models
{
    internal class TileSetDeserializeException : Exception
    {
        public TileSetDeserializeException(Exception exception) : base("Tileset deserialization failed", exception)
        {
        }
    }
}