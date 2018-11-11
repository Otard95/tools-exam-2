using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Domain {
    public static class TileRules
    {
        public static int[] AllowedDimensions = {
            16, 32, 64, 128, 256
        };

        public static bool InvalidTileDimension(int dimension)
        {
            return !AllowedDimensions.Contains(dimension);
        }
    }
}
