using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using LevelEditor.Models;
using LevelEditor.Services;

namespace LevelEditor.ViewModel
{
    public class CanvasViewModel
    {

        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }
        public TileCoordinate LastMouseCoordinate { get; set; }
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public CanvasViewModel()
        {
            SaveAsCommand = new RelayCommand(
                () => FileService.SaveFile(Map, "UntitledMap", "json")
            );
            LoadCommand = new RelayCommand(
                () => FileService.OpenFile("Map", "json", (TileMap map) => Map = map )
            );
        }
        
    }
}
