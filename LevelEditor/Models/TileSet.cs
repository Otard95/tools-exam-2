using System;
using System.Collections.Generic;
using LevelEditor.Domain;
using LevelEditor.Domain.Exceptions;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileSet : IEquatable<TileSet>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Dimension { get; set; }
        public HashSet<TileKey> TileKeys { get; set; }
        public string ContentPath { get; set; }
        public int MapId { get; set; }

        [JsonConstructor]
        public TileSet(Guid id, string name, int dimension)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            if (TileRules.InvalidTileDimension(dimension))
                throw new TileRuleException($"{dimension} is not a valid tile dimension");
            Dimension = dimension;
            TileKeys = new HashSet<TileKey>();
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