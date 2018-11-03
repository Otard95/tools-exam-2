using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LevelEditor.Models;
using Newtonsoft.Json;

namespace LevelEditor.Services {
    public class TileMapService {
        private static TileMapService _instance;
        public static TileMapService Instance => _instance ?? (_instance = new TileMapService());
        public Dictionary<string, TileMap> Factory { get; set; }

        private TileMapService()
        {
            Factory = new Dictionary<string, TileMap>();          
        }

        public TileMap LoadMap(string path)
        {
            if (Factory.TryGetValue(path, out var map))
                return map;

            var tileMap = JsonService.LoadGet<TileMap>(path);
            Factory.Add(path, tileMap);
            return tileMap;
        }
    }

    public class TileMap
    {
        public int Dimension { get; set; }
        public List<TileSet> TileSets { get; set; }
        public Dictionary<int, int> TileSetMap { get; set; }
        public List<TileCoordinate> CoordinateMap { get; set; }
        public List<TileMapping> TileMappings { get; set; }
        public Dictionary<int, int> TilePlacements { get; set; }

        [JsonConstructor]
        public TileMap(int dimension)
        {
            Dimension = dimension;
            TileSets = new List<TileSet>();
            TileSetMap = new Dictionary<int, int>();
            CoordinateMap = new List<TileCoordinate>();
            TileMappings = new List<TileMapping>();
            TilePlacements = new Dictionary<int, int>();
        }

        public void PlaceTile(int x, int y, TileSet tileSet, TileKey tileKey) {
            CheckIfTileSetHasTileDefined(tileSet, tileKey);

            var tileCoordinate = MapTileCoordinate(x, y);
            var coordinateIndex = FindCoordinateIndex(tileCoordinate);

            var tileSetMapId = FindTileSetMapping(tileSet);
            var tileMapping = CreateTileMapping(tileSetMapId, tileKey);
            var tileMappingIndex = FindTileMappingIndex(tileMapping);

            if (TilePlacements.ContainsKey(coordinateIndex)) {
                TilePlacements[coordinateIndex] = tileMappingIndex;
            }
            else {
                TilePlacements.Add(coordinateIndex, tileMappingIndex);
            }
        }

        public TileKey GetTileKey(TileMapping tileMapping, TileSet tileSet) {
            var tileKey = tileSet.TileKeys.FirstOrDefault(key => key.Id == tileMapping.TileId) ??
                          throw new TileMapLogicException($"TileKey with id: {tileMapping.TileId} undefined");
            return tileKey;
        }

        public TileSet GetTileSetFromMapping(TileMapping tileMapping) {
            if (!TileSetMap.ContainsKey(tileMapping.TileSetMapId))
                throw new TileMapLogicException($"TileSetMapId not defined: {tileMapping.TileSetMapId}");
            var tileSetId = TileSetMap[tileMapping.TileSetMapId];

            var tileSet = TileSets.Count > tileSetId-1
                ? TileSets[tileSetId-1]
                : throw new TileMapLogicException($"TileSet with id: {tileSetId} undefined");
            return tileSet;
        }

        private int FindTileMappingIndex(TileMapping tileMapping) {
            if (!TileMappings.Contains(tileMapping))
            {
                throw new TileMapLogicException($"TileMapping not defined: {tileMapping}");
            }

            return TileMappings.IndexOf(tileMapping);
        }

        private int FindTileSetMapping(TileSet tileSet)
        {
            return !TileSetMap.TryGetValue(tileSet.Id, out var tileSetMapId) ? MapTileSet(tileSet) : tileSetMapId;
        }

        private static void CheckIfTileSetHasTileDefined(TileSet tileSet, TileKey tileKey) {
            if (!tileSet.TileKeys.Contains(tileKey))
                throw new TileMapLogicException($"TileSet does not contain a tile with id: {tileKey.Id}");
        }

        private int FindCoordinateIndex(TileCoordinate tileCoordinate) {
            if(!CoordinateMap.Contains(tileCoordinate))
                throw new TileMapLogicException($"TileCoordinate, {tileCoordinate}, not defined! Create a definition first.");
            return CoordinateMap.IndexOf(tileCoordinate);
        }

        private TileCoordinate MapTileCoordinate(int x, int y) {
            var tileCoordinate = new TileCoordinate(x, y);
            if (CoordinateMap.Contains(tileCoordinate)) {
                var index = CoordinateMap.IndexOf(tileCoordinate);
                CoordinateMap[index] = tileCoordinate;
            }
            else {
                CoordinateMap.Add(tileCoordinate);
            }

            return tileCoordinate;
        }

        private TileMapping CreateTileMapping(int tileSetMapId, TileKey tileKey) {
            var tileMapping = new TileMapping {
                TileId = tileKey.Id,
                TileSetMapId = tileSetMapId
            };
            if (!TileMappings.Contains(tileMapping)) {
                TileMappings.Add(tileMapping);
            }

            return tileMapping;
        }

        private int MapTileSet(TileSet tileSet) {

            if (tileSet.Dimension != Dimension)
                throw new TileMapLogicException("TileSet dimension must match TileMap dimension");

            if (TileSets.Contains(tileSet))
                return TileSetMap[tileSet.Id];

            TileSets.Add(tileSet);
            var mapId = GetNextTileSetMapId();
            TileSetMap.Add(tileSet.Id, mapId);
            return TileSetMap[tileSet.Id];
        }

        private int GetNextTileSetMapId() {
            return TileSetMap.Count > 0 ? TileSetMap.Values.Max() + 1 : 1;
        }

        public TileSet GetTileSetForCoordinate(int x, int y) {
            var tileMapping = GetTileMapping(x, y);
            var tileSetIdMapId = TileSetMap[tileMapping.TileSetMapId];
            return TileSets.FirstOrDefault(ts => ts.Id == tileSetIdMapId);
        }

        public TileMapping GetTileMapping(int x, int y) {
            var tileCoordinate = new TileCoordinate(x, y);
            var coordinateIndex = FindCoordinateIndex(tileCoordinate);
            if(!TilePlacements.TryGetValue(coordinateIndex, out var tileMappingIndex))
                throw new TileMapLogicException($"Coordinate not placed: {tileCoordinate}");
            return TileMappings[tileMappingIndex];
        }

        public void ReplaceTileSet(TileSet oldTileSet, TileSet newTileSet)
        {
            if (!TileSetMap.ContainsKey(oldTileSet.Id))
                throw new TileMapLogicException("Cannot replace non-existing Tileset");
            MapTileSet(newTileSet);
            TileSetMap[oldTileSet.Id] = newTileSet.Id;
        }


    }

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
