using System;
using System.Collections.Generic;
using System.Linq;
using LevelEditor.Services;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileMap
    {
        public int Dimension { get; set; }
        public TileSetDictionary TileSetMap { get; set; }
        public List<TileCoordinate> CoordinateMap { get; set; }
        public List<TileMapping> TileMappings { get; set; }
        public Dictionary<int, int> TilePlacements { get; set; }

        [JsonConstructor]
        public TileMap(int dimension)
        {
            Dimension = dimension;
            TileSetMap = new TileSetDictionary();
            CoordinateMap = new List<TileCoordinate>();
            TileMappings = new List<TileMapping>();
            TilePlacements = new Dictionary<int, int>();
        }

        public void PlaceTile(int x, int y, TileSet tileSet, TileKey tileKey) {
            CheckIfTileSetHasTileDefined(tileSet, tileKey);

            var tileCoordinate = MapTileCoordinate(x, y);
            var coordinateIndex = FindCoordinateIndex(tileCoordinate);

            if(!TileSetMap.TryGetTileSetMapping(tileSet.Id, out var mappedTileSet))
            {
                mappedTileSet = TileSetMap.MapNewTileSet(tileSet);
            }

            var tileMapping = CreateTileMapping(mappedTileSet.MapId, tileKey);
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
            return TileSetMap[tileMapping.TileSetMapId];
        }

        private int FindTileMappingIndex(TileMapping tileMapping) {
            if (!TileMappings.Contains(tileMapping))
            {
                throw new TileMapLogicException($"TileMapping not defined: {tileMapping}");
            }

            return TileMappings.IndexOf(tileMapping);
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

        /// <summary>
        /// Maps a TileSet to the TileSet Dictionary and returns a mapping-id to be used with tile-mappings
        /// </summary>
        /// <param name="tileSet">The TileSet Dictionary Map Id</param>
        /// <returns></returns>
        private int MapTileSet(TileSet tileSet) {

            if (tileSet.Dimension != Dimension)
                throw new TileMapLogicException("TileSet dimension must match TileMap dimension");

            TileSetMap[tileSet.Id] = tileSet;
            return TileSetMap[tileSet.Id].MapId;
        }

        public TileMapping GetTileMapping(int x, int y) {
            var tileCoordinate = new TileCoordinate(x, y);
            var coordinateIndex = FindCoordinateIndex(tileCoordinate);
            if(!TilePlacements.TryGetValue(coordinateIndex, out var tileMappingIndex))
                throw new TileMapLogicException($"Coordinate not placed: {tileCoordinate}");
            return TileMappings[tileMappingIndex];
        }


    }
}