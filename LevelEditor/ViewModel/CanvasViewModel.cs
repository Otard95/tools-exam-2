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
using LevelEditor.Views;

namespace LevelEditor.ViewModel
{
    public class CanvasViewModel : ViewModelBase
    {
        private const string DefaultFileName = "Map";
        private const string DefaultTileSetName = "TileSet";
        private string _savedFileName;
        private TileCoordinate _lastMouseCoordinate;
        private MapToolState _state;
        private RelayCommand _previousToolCommand;
        private TileSet _selectedTileSet;
        private int _selectedTile;
        private TileCoordinate _lastTileSetMouseCoordinate;
        private int _tileSetWidth;
        private int _tileSetHeight;
        public Canvas Canvas { get; set; }
        public TileMap Map { get; set; }
        public int[] Dimensions => TileDimensionRules.AllowedDimensions;
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand CreateNewMapCommand { get; set; }
        public RelayCommand ImportTileSetCommand { get; set; }
        public RelayCommand CreateNewTileSetCommand { get; set; }
        public RelayCommand SelectPlaceToolCommand { get; set; }
        public RelayCommand SelectEraserToolCommand { get; set; }

        public int Dimension
        {
            get => Map.Dimension;
            set
            {
                Map.Dimension = value;
                RaisePropertyChanged(nameof(Dimension));
            }
        }

        public MapToolState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        public TileSet SelectedTileSet
        {
            get => _selectedTileSet;
            set => Set(ref _selectedTileSet, value);
        }

        public int SelectedTileId
        {
            get => _selectedTile;
            set => Set(ref _selectedTile, value);
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

        public TileCoordinate LastTileSetMouseCoordinate
        {
            get => _lastTileSetMouseCoordinate;
            set => Set(ref _lastTileSetMouseCoordinate, value);
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

        public Canvas TileSetCanvas { get; set; }
        public TileCoordinate SelectedTileSetTilePosition { get; set; }

        public TileSet[] TileSets => TileSetService.Instance.GetAllTileSets().ToList().Where(ts => ts.Dimension == Dimension).ToArray();

        public int TileSetWidth
        {
            get => _tileSetWidth;
            set => Set(ref _tileSetWidth, value);
        }

        public int TileSetHeight
        {
            get => _tileSetHeight;
            set => Set(ref _tileSetHeight, value);
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
                    SelectedTileSet = Map.TileSetMap.TileSets.FirstOrDefault();
                    Map.TileSetMap.TileSets.ForEach(TileSetService.Instance.AddTileSet);
                    RaisePropertyChanged(nameof(TileSets));
                })
            );
            CreateNewMapCommand = new RelayCommand(
                () =>
                {

                    var dialog = new CreateMapDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        Dimension = dialog.Dimension;
                        Clear();
                    }
                }
            );
            ImportTileSetCommand = new RelayCommand(
                () => FileService.OpenFile(DefaultTileSetName, FileExtension.Json, (TileSet tileSet, string fullFilePath) =>
                {
                    Map.TileSetMap.MapNewTileSet(tileSet);
                    SelectedTileSet = tileSet;
                })
            );
            CreateNewTileSetCommand = new RelayCommand(
                () =>
                {
                    var tileSetEditor = new TilesetEditorWindow();
                    tileSetEditor.Show();
                }
            );

            TileSetService.Instance.Subscribe(() => RaisePropertyChanged(nameof(TileSets)));

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
            if (SelectedTileSet == null) return;
            var tileSet = SelectedTileSet;
            var tileKey = tileSet.TileKeys.FirstOrDefault(tk => tk.Id == SelectedTileId);
            if (tileKey == null) return;
            Map.PlaceTile(x, y, tileSet, tileKey);
        }

        private void Clear()
        {
            Map = new TileMap(Dimension, 20, 20);
            SelectedTileSet = null;
            RaisePropertyChanged(nameof(TileSets));
            RaisePropertyChanged(nameof(TileSets));
        }
    }
}
