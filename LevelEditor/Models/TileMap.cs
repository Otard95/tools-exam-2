using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using LevelEditor.Services;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileMap
    {
        public int Dimension { get; set; }
        public List<TileSet> TileSets { get; set; }
        public Dictionary<int, int> TileSetMap { get; set; }
        public List<TileCoordinate> CoordinateMap { get; set; }
        public List<TileMapping> TileMappings { get; set; }
        public Dictionary<int, int> TilePlacements { get; set; }
        public int Rows { get; set; }
        public int MaxHorizontal { get; set; }
        public int MaxVertical { get; set; }
        public int Columns { get; set; }

        [JsonConstructor]
        public TileMap(int dimension, int rows, int columns)
        {
            Dimension = dimension;
            Columns = columns;
            Rows = rows;
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

            if (tileCoordinate.X > MaxHorizontal)
            {
                MaxHorizontal = tileCoordinate.X;
            }

            if (tileCoordinate.Y > MaxVertical)
            {
                MaxVertical = tileCoordinate.Y;
            }

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

        public void EraseTile(int x, int y)
        {
            var tileCoordinate = new TileCoordinate(x, y);
            CoordinateMap.Remove(tileCoordinate);
        }
    }
}