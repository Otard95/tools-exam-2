using System;
using System.Dynamic;
using System.Windows.Shapes;
using LevelEditor.Domain;
using Newtonsoft.Json;

namespace LevelEditor.Models
{
    public class TileKey : FileKey {

        public TileType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        [JsonIgnore]
        public override string ContentPath { get; set; }
        [JsonIgnore]
        public Guid TileSetId;
        [JsonIgnore]
        public int Dimension { get; set; }
    }
}