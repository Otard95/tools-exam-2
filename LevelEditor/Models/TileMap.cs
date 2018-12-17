using System;
using System.Collections.Generic;
using System.Linq;
using LevelEditor.Domain;
using LevelEditor.Domain.Exceptions;
using LevelEditor.Services;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileMap
    {
        public int Dimension { get; set; }
        public TileSetDictionary TileSetMap { get; set; }
        public List<TileCoordinate> CoordinateMap { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        [JsonConstructor]
        public TileMap(int dimension, int rows, int columns)
        {
            Dimension = dimension;
            Rows = rows;
            Columns = columns;
            TileSetMap = new TileSetDictionary();
            CoordinateMap = new List<TileCoordinate>();
        }

        public void PlaceTile(int x, int y, TileSet tileSet, TileKey tileKey) {

            if (tileSet.Dimension != Dimension)
                throw new TileMapLogicException("TileSet dimension must match TileMap dimension");

            CheckIfTileSetHasTileDefined(tileSet, tileKey);
            if (!TileSetMap.TryGetTileSetMapping(tileSet.Id, out var mappedTileSet)) {
                mappedTileSet = TileSetMap.MapNewTileSet(tileSet);
            }

            PlaceTileMapping(x, y, mappedTileSet.MapId, tileKey.Id);

        }

        public TileKey GetTileKey(TileCoordinate tileCoordinate, TileSet tileSet)
        {
            var tileKey = tileSet.TileKeys.FirstOrDefault(key => key.Id == tileCoordinate.TileId) ??
                          throw new TileMapLogicException(
                              $"Tile with ID, {tileCoordinate.TileId} is not mapped in tileset: {tileSet.Id}");
            return tileKey;
        }

        private static void CheckIfTileSetHasTileDefined(TileSet tileSet, TileKey tileKey) {
            if (!tileSet.TileKeys.Contains(tileKey))
                throw new TileMapLogicException($"TileSet does not contain a tile with id: {tileKey.Id}");
        }

        private void PlaceTileMapping(int x, int y, int tileSetId, int tileId) {
            var tileCoordinate = new TileCoordinate(x, y, tileSetId, tileId);
            if (CoordinateMap.Contains(tileCoordinate)) {
                var index = CoordinateMap.IndexOf(tileCoordinate);
                CoordinateMap[index] = tileCoordinate;
            }
            else {
                CoordinateMap.Add(tileCoordinate);
            }
        }

        public void EraseTile(int x, int y) {
            var tileCoordinate = new TileCoordinate(x, y, 0, 0);
            CoordinateMap.Remove(tileCoordinate);
        }

    }
}