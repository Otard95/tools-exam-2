using System;

namespace LevelEditor.Models
{
    public class TileCoordinate : IEquatable<TileCoordinate>
    {
        public TileCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

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

        public override int GetHashCode()
        {
            return $"{X},{Y}".GetHashCode();
        }

    }
}