using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    internal class TileSetDeserializeException : Exception
    {
        public TileSetDeserializeException(Exception exception) : base("TileSetImageSource deserialization failed", exception)
        {
        }
    }
}