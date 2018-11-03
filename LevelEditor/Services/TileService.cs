using System.Collections.Generic;
using LevelEditor.Models;

namespace LevelEditor.Services {
    public class TileService {
        private static TileService _instance;

        public static TileService Instance => _instance ?? (_instance = new TileService());
        public Dictionary<string, TileSet> Factory { get; set; }

        private TileService()
        {
            Factory = new Dictionary<string, TileSet>();
        }

        public TileSet GetTileset(string path)
        {
            if (Factory.TryGetValue(path, out var tileSet))
                return tileSet;

            tileSet = JsonService.LoadGet<TileSet>(path);
            Factory.Add(path, tileSet);
            return tileSet;
        }
    }
}
