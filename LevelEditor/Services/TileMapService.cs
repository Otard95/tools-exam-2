using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LevelEditor.Models;

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
}
