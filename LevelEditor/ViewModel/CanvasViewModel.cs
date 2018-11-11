using System;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
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
        private MapToolState _state;
        private RelayCommand _previousToolCommand;
        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }

        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SelectPlaceToolCommand { get; set; }
        public RelayCommand SelectEraserToolCommand { get; set; }

        public MapToolState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        public void SelectMapTool(MapToolState state, [CallerMemberName] string propertyName = null)
        {
            State = state;
            if (string.IsNullOrEmpty(propertyName)) return;
            if (!(GetType().GetProperty(propertyName)?.GetValue(this) is RelayCommand command)) return;
            _previousToolCommand?.RaiseCanExecuteChanged();
            command.RaiseCanExecuteChanged();
            _previousToolCommand = command;
        }

        public TileCoordinate LastMouseCoordinate {
            get => _lastMouseCoordinate;
            set => Set(ref _lastMouseCoordinate, value);
        }

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

        public CanvasViewModel() {
            Map = new TileMap(128, 20, 20);

            SaveAsCommand = new RelayCommand(
                () => FileService.SaveFileAs(Map, DefaultFileName, FileExtension.Json, fullFilePath => SavedFileName = fullFilePath)
            );
            SaveCommand = new RelayCommand(
                () => FileService.SaveFile(Map, SavedFileName),
                canExecute: () => !string.IsNullOrEmpty(SavedFileName)
            );
            LoadCommand = new RelayCommand(
                () => FileService.OpenFile(DefaultFileName, FileExtension.Json, (TileMap map, string fullFilePath) => {
                    Map = map;
                    SavedFileName = fullFilePath;
                })
            );

            InitializeToolState();
        }

        private void InitializeToolState() {
            State = MapToolState.Place;
            SelectPlaceToolCommand = new RelayCommand(
                () => SelectMapTool(MapToolState.Place, nameof(SelectPlaceToolCommand)),
                canExecute: () => State != MapToolState.Place
            );
            SelectEraserToolCommand = new RelayCommand(
                () => SelectMapTool(MapToolState.Eraser, nameof(SelectEraserToolCommand)),
                canExecute: () => State != MapToolState.Eraser
            );
            _previousToolCommand = SelectPlaceToolCommand;
        }

        public void EditMap(int x, int y)
        {
            switch (State)
            {
                case MapToolState.Place: PlaceTile(x, y);
                    break;
                case MapToolState.Eraser: Map.EraseTile(x, y);
                break;
            }
        }

        private void PlaceTile(int x, int y)
        {
            var tileSet = Map.TileSetMap.TileSets.First();
            var tileKey = tileSet.TileKeys.First();
            Map.PlaceTile(x, y, tileSet, tileKey);
        }



    }
}
