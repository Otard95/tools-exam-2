using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Factories
{
    public class TileFactory {
        private static TileFactory _tileFactory;
        public static TileFactory Instance {
            get => _tileFactory ?? (_tileFactory = new TileFactory());
            private set => _tileFactory = value;
        }

        private TileFactory() {

        }

        internal void LoadTileSet () {
            
        }
    }
}
