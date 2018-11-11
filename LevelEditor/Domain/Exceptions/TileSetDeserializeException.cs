using System;

namespace LevelEditor.Domain.Exceptions
{
    internal class TileSetDeserializeException : Exception
    {
        public TileSetDeserializeException(Exception exception) : base("TileSetImageSource deserialization failed", exception)
        {
        }
    }
}