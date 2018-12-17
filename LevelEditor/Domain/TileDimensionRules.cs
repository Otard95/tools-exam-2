using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Domain {
    public static class TileDimensionRules
    {
        public static readonly int[] AllowedDimensions =
        {
            32, 64, 128, 256
        };

        public static int MinDimension => AllowedDimensions.Min();
        public static int MaxDimension => AllowedDimensions.Max();
        public static int NumOfDimensions => AllowedDimensions.Length;
    }
}
