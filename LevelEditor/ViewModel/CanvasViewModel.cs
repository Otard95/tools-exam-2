using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LevelEditor.Domain;
using LevelEditor.Models;
using LevelEditor.Services;

namespace LevelEditor.ViewModel
{
    public class CanvasViewModel : ViewModelBase
    {
        private const string DefaultFileName = "Map";
        private string _savedFileName;
        private TileCoordinate _lastMouseCoordinate;
        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }

        public TileCoordinate LastMouseCoordinate
        {
            get => _lastMouseCoordinate;
            set => Set(ref _lastMouseCoordinate, value);
        }

        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public string SavedFileName
        {
            get => _savedFileName;
            set
            {
                Set(ref _savedFileName, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public int Height => Map.Dimension * Map.Rows;

        public int Width => Map.Dimension * Map.Columns;

        public int Rows
        {
            get => Map.Rows;
            set
            {
                Map.Rows = value;
                RaisePropertyChanged(nameof(Rows));
                RaisePropertyChanged(nameof(Height));
            }
        }

        public int Columns {
            get => Map.Columns;
            set {
                Map.Columns = value;
                RaisePropertyChanged(nameof(Columns));
                RaisePropertyChanged(nameof(Width));
            }
        }

        public CanvasViewModel()
        {
            Map = new TileMap(128, 20, 20);

            SaveAsCommand = new RelayCommand(
                () => FileService.SaveFileAs(Map, DefaultFileName, FileExtension.Json, fullFilePath => SavedFileName = fullFilePath)
            );
            SaveCommand = new RelayCommand(
                () => FileService.SaveFile(Map, SavedFileName), 
                canExecute: () => !string.IsNullOrEmpty(SavedFileName)
            );
            LoadCommand = new RelayCommand(
                () => FileService.OpenFile(DefaultFileName, FileExtension.Json, (TileMap map, string fullFilePath) =>
                {
                    Map = map;
                    SavedFileName = fullFilePath;
                })
            );
        }
    }
}
