using LevelEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Services {
    public class TilesetService {

        private static TilesetService _instance;
        public static TilesetService Instance => _instance ?? (_instance = new TilesetService());

        private Dictionary<string, TileSet> _tilesets;

        private TilesetService () {
            _tilesets = new Dictionary<string, TileSet>();
        }

        public TileSet GetTileset (string id) {
            if (_tilesets.TryGetValue(id, out var tileset))
                return tileset;

            throw new ArgumentException($"No tileset with id: {id}");
        }

        public TileSet[] GetAllTilesets () {
            return _tilesets.Values.ToArray();
        }

    }
}
