using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LevelEditor.Domain;
using LevelEditor.Domain.Exceptions;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileSet : IEquatable<TileSet>
    {
        [JsonIgnore]
        private Guid _id;
        [JsonIgnore]
        private string _contentPath;
        [JsonIgnore]
        private int _dimension;

        public Guid Id
        {
            get => _id;
            private set
            {
                _id = value;
                MapTileInformation();
            }
        }

        public string Name { get; set; }

        public int Dimension
        {
            get => _dimension;
            set
            {
                _dimension = value;
                MapTileInformation();
            }
        }

        public int TileIdCount { get; set; }
        public HashSet<TileKey> TileKeys { get; set; }

        public string ContentPath
        {
            get => _contentPath;
            set
            {
                _contentPath = value;
                MapTileInformation();
            }
        }

        public int MapId { get; set; }

        [JsonConstructor]
        public TileSet(Guid id, string name, int dimension, string contentPath)
        {
            TileKeys = new HashSet<TileKey>();
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            if (TileRules.InvalidTileDimension(dimension))
                throw new TileRuleException($"{dimension} is not a valid tile dimension");
            _dimension = dimension;
            _contentPath = contentPath;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context) {
            MapTileInformation();
        }

        private void MapTileInformation() {
            foreach (var tileKey in TileKeys) {
                tileKey.ContentPath = ContentPath;
                tileKey.TileSetId = Id;
                tileKey.Dimension = Dimension;
            }
        }

        public void AddTile(TileKey tile)
        {
            tile.ContentPath = ContentPath;
            tile.Dimension = Dimension;
            tile.TileSetId = Id;
            tile.Id = ++TileIdCount;
            TileKeys.Add(tile);
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

        public override string ToString()
        {
            return Name;
        }

        internal void Clear () {
            TileKeys.Clear();
            TileIdCount = 0;
        }
    }
}