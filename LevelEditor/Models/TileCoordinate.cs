using System;
using System.CodeDom;
using System.Linq;

namespace LevelEditor.Models
{
    public class TileCoordinate : IEquatable<TileCoordinate>
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public TileCoordinate(int x, int y, int tileSetMapId, int tileId)
        {
            X = x;
            Y = y;
            TileSetMapId = tileSetMapId;
            TileId = tileId;
        }

        public int TileId { get; set; }
        public int TileSetMapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(TileCoordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TileCoordinate) obj);
        }

        public static bool operator ==(TileCoordinate a, TileCoordinate b)
        {
            return a?.Equals(b) ?? false;
        }

        public static bool operator !=(TileCoordinate a, TileCoordinate b)
        {
            return !(a == b);
        }
    }
}