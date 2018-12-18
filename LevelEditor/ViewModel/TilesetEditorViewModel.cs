using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Models;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using LevelEditor.Domain;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : ViewModelBase
    {
        private TileSet _tileSet;
        private int _selectedTileSet;
        private readonly OpenFileDialog _fileDialog;
        private int _dimension;
        private ImageSource _selectedTileImage;
        private int _selectedTileId;
        private TileCoordinate _selectedTilePosition;

        public ICommand BrowseCommand { get; }
        public RelayCommand SliceCommand { get; }
         
        #region UI property bindings

        public BitmapSource TileSetImageSource {
            get {
                if (TileSet == null) return null;
                return BitmapService.Instance.GetBitmapSource(TileSet.ContentPath);
            }
        }

        public ImageSource SelectedTileImage
        {
            get => _selectedTileImage;
            set => Set(ref _selectedTileImage, value);
        }

        public string TilesetName {
            get => TileSet != null ? TileSet.Name : string.Empty;
            set {
                if (TileSet != null) TileSet.Name = value;
                RaisePropertyChanged(nameof(TilesetName));
                SliceCommand?.RaiseCanExecuteChanged();
            }
        }

        public int Dimension
        {
            get => _dimension;
            set {
                Set(ref _dimension, value);
                RaisePropertyChanged(nameof(Dimension));
                SliceCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool CanEditTileProperties => SelectedTilePosition != null;
        public int MaxDimension => TileDimensionRules.NumOfDimensions-1;

        public TileSet TileSet {
            get => _tileSet;
            private set {
                Set(ref _tileSet, value);
                Dimension = _tileSet.Dimension;
                RaisePropertyChanged(nameof(TilesetName));
            }
        }
        public List<string> ExistingTilesets => TileSetService.Instance.GetAllTileSets().Select(ts => ts.Name).ToList();

        public int SelectedTileSet {
            get => _selectedTileSet;
            set {
                Set(ref _selectedTileSet, value);
                if (value > -1) TileSet = TileSetService.Instance.GetAllTileSets()[_selectedTileSet];
                RaisePropertyChanged(nameof(SelectedTileSet));
            }
        }

        public bool SelectedTileIsWalkable {
            get {
                var value = TileSet?.TileKeys.FirstOrDefault(ts => ts.Id == SelectedTileId)?.Walkable ?? false;
                return value;
            }
            set
            {
                var key = TileSet?.TileKeys.FirstOrDefault(ts => ts.Id == SelectedTileId);
                if (key == null) return;
                key.Walkable = value;
                RaisePropertyChanged(nameof(SelectedTileIsWalkable));
            }
        }

        public int SelectedTileLayer {
            get {
                return TileSet?.TileKeys.FirstOrDefault(ts => ts.Id == SelectedTileId)?.Layer ?? 0;
            }
            set {
                var key = TileSet?.TileKeys.FirstOrDefault(ts => ts.Id == SelectedTileId);
                if (key == null) return;
                key.Layer = value;
                RaisePropertyChanged(nameof(SelectedTileLayer));
            }
        }


        public TileCoordinate LastMouseCoordinate { get; set; }

        public TileCoordinate SelectedTilePosition
        {
            get => _selectedTilePosition;
            set
            {
                Set(ref _selectedTilePosition, value);
                RaisePropertyChanged(nameof(CanEditTileProperties));
            }
        }

        public int SelectedTileId
        {
            get => _selectedTileId;
            set
            {
                Set(ref _selectedTileId, value);
                RaisePropertyChanged(nameof(SelectedTileIsWalkable));
                RaisePropertyChanged(nameof(SelectedTileLayer));
            }
        }

        #endregion

        public TilesetEditorViewModel () {
            Dimension = 128;

            _fileDialog = new OpenFileDialog {Filter = $"Image File|*.{FileExtension.Png};*.{FileExtension.Jpg};*.{FileExtension.Bmp}"};

            BrowseCommand = new RelayCommand(StartBrowse);
            SliceCommand =
                new RelayCommand(SliceTileSet, CanSlice);

        }

        private void StartBrowse () {
            if (_fileDialog.ShowDialog() != true) return;

            var tilesetFromFile = BitmapService.Instance.GetBitmapSource(_fileDialog.FileName);

            var heightIsDividableByTwo = IsDividableByTwo(tilesetFromFile.PixelHeight);
            var widthIsDividableByTwo = IsDividableByTwo(tilesetFromFile.PixelWidth);
            if (!heightIsDividableByTwo || !widthIsDividableByTwo) {
                MessageBox.Show($"The image you selected doesn't have the right dimension", "Oops", MessageBoxButton.OK);
                return;
            }

            var tileSet = TileSetService.Instance.GetTileSetByContentPath(_fileDialog.FileName);
            if (tileSet == null) {
                _tileSet = new TileSet(Guid.Empty, "New Tileset", Dimension, _fileDialog.FileName);
                _selectedTileSet = -1;
            } else {
                _dimension = TileSet.Dimension;
                _selectedTileSet = ExistingTilesets.IndexOf(TileSet.Name);
                _tileSet = tileSet;
            }

            RaisePropertyChangedAll();

        }

        private bool IsDividableByTwo(int number)
        {
            return (number & ((1 << 2) - 1)) == 0;
        }

        private bool CanSlice () {
            return TileSetImageSource != null && Dimension != 0 && !string.IsNullOrEmpty(TilesetName) &&
                   TileSetImageSource.PixelHeight % Dimension == 0 &&
                   TileSetImageSource.PixelWidth % Dimension == 0;
        }

        public void SliceTileSet()
        {
            TileSet.Clear();
            TileSet.Dimension = Dimension;
            var width = TileSetImageSource.PixelWidth;
            var height = TileSetImageSource.PixelHeight;
            var rowCount = height / Dimension;
            var columnCount = width / Dimension;

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    TileSet.AddTile(new TileKey
                    {
                        X = column,
                        Y = row
                    });
                }
            }

            try {
                TileSetService.Instance.GetTileSet(TileSet.Id);
            } catch (Exception e) {
                if (TileSetService.Instance.AddTileSet(TileSet)) {
                    MessageBox.Show($"The new tileset '{TilesetName}' was saved successfully.", "Success", MessageBoxButton.OK);
                    SelectedTileSet = ExistingTilesets.IndexOf(TilesetName);
                    RaisePropertyChangedAll();

                } else {
                    MessageBox.Show($"The tileset couldn't be saved. There might be a tileset already named '{TilesetName}'.", "Oops", MessageBoxButton.OK);
                }
            }
            
        }

        private void RaisePropertyChangedAll () {
            RaisePropertyChanged(nameof(ExistingTilesets));
            RaisePropertyChanged(nameof(TileSet));
            RaisePropertyChanged(nameof(SelectedTileSet));
            RaisePropertyChanged(nameof(TilesetName));
            RaisePropertyChanged(nameof(Dimension));
            
        }

    }

}
