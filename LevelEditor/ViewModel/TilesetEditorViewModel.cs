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

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : ViewModelBase
    {
        private TileSet _tileSet;
        private int _selectedTileSet;
        private readonly OpenFileDialog _fileDialog;
        private SliceType _sliceType;
        private int _dimension;

        public ICommand BrowseCommand { get; private set; }
        public RelayCommand SliceCommand { get; private set; }

        #region UI property bindings

        public BitmapSource TileSetImageSource {
            get {
                if (TileSet == null) return null;
                return BitmapService.Instance.GetBitmapSource(TileSet.ContentPath);
            }
        }

        public string TilesetName {
            get {
                return TileSet != null ? TileSet.Name : "";
            }
            set {
                if (TileSet != null) TileSet.Name = value;
                RaisePropertyChanged(nameof(TilesetName));
                SliceCommand?.RaiseCanExecuteChanged();
            }
        }

        public string[] SliceModeChoices => Enum.GetNames(typeof(SliceType));

        public int SelectedSliceMode
        {
            get => (int) _sliceType;
            set => _sliceType = (SliceType) value;
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

        public int MaxDimension => TileDimensionRules.NumOfDimensions-1;

        public TileSet TileSet {
            get => _tileSet;
            private set {
                Set(ref _tileSet, value);
                Dimension = _tileSet.Dimension;
                RaisePropertyChanged(nameof(TileSet));
                RaisePropertyChanged(nameof(TilesetName));
            }
        }
        public List<string> ExistingTilesets { get => TileSetListToNames(TileSetService.Instance.GetAllTileSets()); }
        public int SelectedTileSet {
            get => _selectedTileSet;
            set {
                Set(ref _selectedTileSet, value);
                if (value > -1) TileSet = TileSetService.Instance.GetAllTileSets()[_selectedTileSet];
                RaisePropertyChanged(nameof(SelectedTileSet));
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

        private List<string> TileSetListToNames (TileSet[] tileSets) {
            var output = new List<string>();
            foreach (var tileset in tileSets) {
                output.Add(tileset.Name);
            }
            return output;
        }

        private void StartBrowse () {
            if (_fileDialog.ShowDialog() != true) return;

            // Review: We should probably be more specific with the naming.
            var tilesetFromFile = BitmapService.Instance.GetBitmapSource(_fileDialog.FileName);

            // Make sure image size is divisable by 2
            var a = (tilesetFromFile.PixelHeight & ((1 << 2) - 1)) != 0;
            var b = (tilesetFromFile.PixelWidth & ((1 << 2) - 1)) != 0;
            if (a || b) {
                MessageBox.Show($"The image you selected doesn't have the right dimentions", "Oops", MessageBoxButton.OK);
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
            var rowCount = (int) height / Dimension;
            var columnCount = (int) width / Dimension;

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
                    MessageBox.Show($"The tileset couldn't be saved. There might be a tileset allready named '{TilesetName}'.", "Oops", MessageBoxButton.OK);
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
