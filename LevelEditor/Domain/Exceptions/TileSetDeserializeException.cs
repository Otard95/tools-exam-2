using System;

namespace LevelEditor.Models
{
    internal class TileSetDeserializeException : Exception
    {
        public TileSetDeserializeException(Exception exception) : base("TilesetImageSource deserialization failed", exception)
        {
        }
    }
}