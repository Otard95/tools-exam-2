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

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : ViewModelBase
    {
        private readonly TileSet _tileSet;
        private readonly OpenFileDialog _fileDialog;
        private SliceType _sliceType;
        private string _workingFile;
        private string PrevWorkingFile { get; set; }
        private int _sizeExp;
        private int _dimension;

        public ICommand BrowseCommand { get; private set; }
        public RelayCommand SliceCommand { get; private set; }

        public Canvas Canvas { get; set; }

        #region UI property bindings

        public BitmapSource TileSetImageSource => BitmapService.Instance.GetBitmapSource(WorkingFile);

        public string WorkingFile {
            get => _workingFile;
            private set
            {
                Set(ref _workingFile, value);
                RaisePropertyChanged(nameof(TileSetImageSource));
                SliceCommand.RaiseCanExecuteChanged();
            }
        }

        public string TilesetName { get; set; }

        public string[] SliceModeChoices => Enum.GetNames(typeof(SliceType));

        public int SelectedSliceMode
        {
            get => (int) _sliceType;
            set => _sliceType = (SliceType) value;
        }

        public int Dimension
        {
            get => _dimension;
            set => Set(ref _dimension, value);
        }

        public int MaxDimension => TileDimensionRules.NumOfDimensions-1;
        public TileSet TileSet { get; private set; }

        #endregion

        public TilesetEditorViewModel () {
            _sizeExp = 5;
            Dimension = 128;
            TilesetName = "New Tileset";

            _fileDialog = new OpenFileDialog {Filter = $"Image File|*.{FileExtension.Png};*.{FileExtension.Jpg};*.{FileExtension.Bmp}"};

            BrowseCommand = new RelayCommand(StartBrowse);
            SliceCommand =
                new RelayCommand(SliceTileSet, canExecute: () => TileSetImageSource != null && Dimension != 0 && !string.IsNullOrEmpty(TilesetName)); //&&
            //TileSetImageSource.PixelHeight % Dimension == 0 &&
            //TileSetImageSource.Width % Dimension == 0);

        }

        private void StartBrowse () {
            if (_fileDialog.ShowDialog() != true) return;
            WorkingFile = _fileDialog.FileName;
        }

        private void UpdateImageSource () {

            // Review: We should probably be more specific with the naming.
            var tilesetFromFile = BitmapService.Instance.GetBitmapSource(WorkingFile);
            var nx = Math.Log(tilesetFromFile.PixelHeight, 2);
            var hd = (int)nx;
            var ny = Math.Log(tilesetFromFile.PixelWidth, 2);
            var wd = (int)ny;

            if (hd != nx || wd != ny) {
                WorkingFile = PrevWorkingFile;
                return;
            }

            PrevWorkingFile = WorkingFile;
        }

        public void SliceTileSet()
        {
            var tileSet = new TileSet(Guid.Empty, TilesetName, Dimension, WorkingFile);
            var width = TileSetImageSource.PixelWidth;
            var height = TileSetImageSource.PixelHeight;
            var dimension = Dimension;
            var rowCount = (int) height / dimension;
            var columnCount = (int) width / dimension;

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    tileSet.AddTile(new TileKey
                    {
                        X = column,
                        Y = row
                    });
                }
            }

            if (TileSetService.Instance.AddTileSet(tileSet)) {
                MessageBox.Show($"The new tileset '{TilesetName}' was saved successfully.", "Success", MessageBoxButton.OK);
            } else {
                MessageBox.Show($"The tileset couldn't be saved. There might be a tileset allready named '{TilesetName}'.", "Oops", MessageBoxButton.OK);
            }
        }
    }

}
