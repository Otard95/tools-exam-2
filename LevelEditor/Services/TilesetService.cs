using LevelEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Services {
    public class TileSetService {

        private static TileSetService _instance;
        public static TileSetService Instance => _instance ?? (_instance = new TileSetService());

        private readonly Dictionary<Guid, TileSet> _tileSets;
        private readonly List<Action> _subscriptions;

        private TileSetService () {
            _tileSets = new Dictionary<Guid, TileSet>();
            _subscriptions = new List<Action>();
        }

        public bool NameExists (string name) {
            foreach (var tileSet in _tileSets.Values) {
                if (tileSet.Name == name) return true;
            }
            return false;
        }

        public bool Contains (Guid id) {
            if (_tileSets.TryGetValue(id, out var tileSet))
                return true;

            return false;
        }

        public void RemoveTileSet (Guid id) {
            _tileSets.Remove(id);
        }

        public bool AddTileSet(TileSet newTileSet)
        {
            if (NameExists(newTileSet.Name)) return false;
            _tileSets.Add(newTileSet.Id, newTileSet);
            _subscriptions.ForEach(s => s.Invoke());
            return true;
        }

        public TileSet GetTileSet (Guid id) {
            if (_tileSets.TryGetValue(id, out var tileSet))
                return tileSet;

            throw new ArgumentException($"No tileSet with id: {id}");
        }

        public TileSet[] GetAllTileSets () {
            return _tileSets.Values.ToArray();
        }

        public void Subscribe(Action notificationCallback)
        {
            _subscriptions.Add(notificationCallback);
        }
    }
}
