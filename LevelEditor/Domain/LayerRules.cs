using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Domain {
    public static class LayerRules
    {
        public const int MaxLayer = 3;
        public static readonly int[] AllowedLayers = Enumerable.Range(1, MaxLayer).ToArray();
    }
}
