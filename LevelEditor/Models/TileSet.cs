using System;
using System.Collections.Generic;

namespace LevelEditor.Models
{
    public class TileSet : IEquatable<TileSet> {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Dimension { get; set; }
        public HashSet<TileKey> TileKeys { get; set; }
        public int MapId { get; set; }

        public TileSet(string name, int dimention)
        {
            Id = Guid.NewGuid();
            Name = name;
            Dimension = dimention;
        }

        public bool Equals(TileSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TileSet) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}