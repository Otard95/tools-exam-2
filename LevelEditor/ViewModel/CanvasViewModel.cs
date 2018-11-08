using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using LevelEditor.Models;

namespace LevelEditor.ViewModel
{
    public class CanvasViewModel
    {

        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }
        public TileCoordinate LastMouseCoordinate { get; set; }

        public CanvasViewModel() {
            
        }
        
    }
}
