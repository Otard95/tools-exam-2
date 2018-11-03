using System.Collections.Generic;
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
    }

    public class TileMap
    {
        public int Dimension { get; set; }
        public HashSet<TileSet> TileSets { get; set; }
        public Dictionary<int, int> TileSetMap { get; set; }
        public int[][][] TilePlacements { get; set; }

        public void PlaceTile(int x, int y, TileSet tileSet, int tileId) {
            CheckTileSetExistence(tileSet);
            TilePlacements[x][y] = new[] { tileSet.Id, TileSetMap[tileSet.Id] };
        }

        private void CheckTileSetExistence(TileSet tileSet) {

            if (tileSet.Dimension != Dimension)
                throw new TileMapLogicException("TileSet dimension must match TileMap dimension");

            if (!TileSets.Contains(tileSet)) {
                TileSets.Add(tileSet);
                var mapId = TileSetMap.Values.Max() + 1;
                TileSetMap.Add(tileSet.Id, mapId);
            }
        }

        public TileSet GetTileSetForCoordinate(int x, int y)
        {
            var tileDefiniton = TilePlacements[x][y];
            var tileMapId = tileDefiniton[0];
            var tileSetId = TileSetMap[tileMapId];
            return TileSets.FirstOrDefault(ts => ts.Id == tileSetId);
        }

        public BitmapSource GetTileBitMapSourceForCoordinate(int x, int y)
        {
            var tileDefiniton = TilePlacements[x][y];
            var tileId = tileDefiniton[1];
            var tileSet = GetTileSetForCoordinate(x, y);
            return tileSet[new TileKey {Id = tileId}];
        }

        public int GetTileKeyForCoordinate(int x, int y) {
            var tileDefiniton = TilePlacements[x][y];
            return tileDefiniton[1];
        }

        public void ReplaceTileSet(TileSet oldTileSet, TileSet newTileSet)
        {
            if (!TileSetMap.ContainsKey(oldTileSet.Id))
                throw new TileMapLogicException("Cannot replace non-existing Tileset");
            CheckTileSetExistence(newTileSet);
            TileSetMap[oldTileSet.Id] = newTileSet.Id;
        }
    }
}
