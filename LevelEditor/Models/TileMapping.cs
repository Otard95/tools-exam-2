using System;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileMapping : IEquatable<TileMapping>
    {
        public int TileSetMapId { get; set; }
        public int TileId { get; set; }

        public bool Equals(TileMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TileSetMapId == other.TileSetMapId && TileId == other.TileId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TileMapping) obj);
        }

        public override int GetHashCode()
        {
            return $"{TileSetMapId.GetHashCode()},{TileId.GetHashCode()}".GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}