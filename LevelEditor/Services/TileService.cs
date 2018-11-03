using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LevelEditor.Tools;
using Newtonsoft.Json;

namespace LevelEditor.Services {
    public class TileService
    {
        private TileService _instance;

        public TileService Instance => _instance ?? (_instance = new TileService());
        public Dictionary<string, TileSet> Tiles { get; set; }

        public void LoadTilesets(string path)
        {
            Tiles.Add(path, Json.LoadGet<TileSet>(path));
        }

    }

    public class TileSet
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public HashSet<TileKey> TileKeys { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            foreach (var tileKey in TileKeys)
            {
                // TileSetDictionary.Add(TileKey, ); Load files here
            }
        }

        [JsonIgnore]
        private Dictionary<TileKey, ImageSource> TileSetDictionary { get; set; }

        [JsonIgnore]
        public ImageSource this[TileKey key] => TileSetDictionary.TryGetValue(key, out var img)
            ? img
            : throw new TileLoadException($"Could not load tile: {key}");
    }

    public class TileLoadException : Exception
    {
        public TileLoadException(string s) : base(s)
        {

        }
    }

    public class TileKey : IEquatable<TileKey>
    {
        public int Id { get; set; }
        public string ContentPath { get; set; }

        public bool Equals(TileKey other)
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
            return Equals((TileKey) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return $"[{Id}] {ContentPath ?? "NULL"}";
        }
    }
}
