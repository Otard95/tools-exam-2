using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LevelEditor.Domain.Exceptions;
using LevelEditor.Services;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileSet : IEquatable<TileSet> {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Dimension { get; set; }

        public HashSet<TileKey> TileKeys { get; set; }

        public TileSet()
        {
            TileSetDictionary = new Dictionary<TileKey, BitmapSource>();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context) {
            try
            {
                foreach (var tileKey in TileKeys) {
                    var bitmap = new Bitmap(tileKey.ContentPath);
                    var bitmapSource = Converters.BitmapToBitmapSource(bitmap);
                    TileSetDictionary.Add(tileKey, bitmapSource);
                }
            }
            catch (Exception e)
            {
                throw new TileSetDeserializeException(e);
            }
        }

        [JsonIgnore]
        private Dictionary<TileKey, BitmapSource> TileSetDictionary { get; set; }

        [JsonIgnore]
        public BitmapSource this[TileKey key] => TileSetDictionary.TryGetValue(key, out var img)
            ? img
            : throw new TileLoadException($"Could not load tile: {key}");

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
            return Id;
        }
    }
}