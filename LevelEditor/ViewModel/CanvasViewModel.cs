using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LevelEditor.Models;
using LevelEditor.Services;

namespace LevelEditor.ViewModel
{
    public class CanvasViewModel : ViewModelBase
    {
        private int _height;
        private int _width;
        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }
        public TileCoordinate LastMouseCoordinate { get; set; }
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public int Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public int Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        public CanvasViewModel()
        {
            Map = new TileMap(128, 20, 20);
            Height = Map.Dimension * Map.Rows;
            Width = Map.Dimension * Map.Columns;
            SaveAsCommand = new RelayCommand(
                () => FileService.SaveFile(Map, "UntitledMap", "json")
            );
            LoadCommand = new RelayCommand(
                () => FileService.OpenFile("Map", "json", (TileMap map) => Map = map )
            );
        }
        
    }
}
