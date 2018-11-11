using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LevelEditor.Models {
    public class TileSetDictionary
    {
        [JsonIgnore]
        private int _maxId;
        public Dictionary<int, Guid> TileSetMappings { get; set; }
        public List<TileSet> TileSets { get; set; }

        public TileSetDictionary()
        {
            TileSets = new List<TileSet>();
            TileSetMappings = new Dictionary<int, Guid>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (TileSetMappings.Keys.Count > 0)
            {
                _maxId = TileSetMappings.Keys.Max();
            }
        }

        public bool TileSetIsDefined(Guid tileSetId)
        {
            return TileSetMappings.Values.Contains(tileSetId);
        }

        public bool TryGetTileSetMapping(Guid tileSetId, out TileSet tileSet)
        {
            tileSet = null;
            try
            {
                if (TileSetIsDefined(tileSetId))
                    tileSet = this[tileSetId];
            }
            catch (Exception)
            {
                return false;
            }
            return tileSet != null;
        }

        public TileSet MapNewTileSet(TileSet tileSet)
        {
            this[tileSet.Id] = tileSet;
            return this[tileSet.Id];
        }

        [JsonIgnore]
        public TileSet this[int key] {
            get
            {
                var guid = TileSetMappings[key];
                return this[guid];
            }
        }

        [JsonIgnore]
        public TileSet this[Guid key]
        {
            get
            {
                var searchObj = new TileSet(key, "", TileSets.First().Dimension, "");
                var index = TileSets.IndexOf(searchObj);
                return TileSets[index];
            }
            set
            {
                var newTileSet = value; 
                if (newTileSet == null)
                    throw new NullReferenceException();
                if(newTileSet.Id != key)
                    throw new TileSetAmbiguityException($"TileSetId must match dictionary key");
                if (MapIdNotSet(newTileSet)) {
                    GetNextId(newTileSet);
                }

                if (TileSets.Contains(newTileSet)) { //Hash lookup
                    ReplaceTileSet(newTileSet);
                }
                else {
                    TileSets.Add(newTileSet);
                    AddNewTileSet(newTileSet);
                }

            }
        }

        private void AddNewTileSet(TileSet value) {
            TileSetMappings.Add(value.MapId, value.Id);
        }

        private void ReplaceTileSet(TileSet value) {
            var index = TileSets.IndexOf(value);
            var oldTileSet = TileSets[index];
            if (value.MapId != oldTileSet.MapId)
                throw new TileSetAmbiguityException("MapId mismatch between an existing tile-set and the one you're trying to add.");
            TileSets[index] = value;
        }

        private void GetNextId(TileSet value) {
            value.MapId = ++_maxId;
        }

        private static bool MapIdNotSet(TileSet value) {
            return value.MapId == 0;
        }
    }
}
