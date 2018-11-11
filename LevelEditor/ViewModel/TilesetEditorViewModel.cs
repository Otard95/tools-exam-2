using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Models;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : ViewModelBase
    {

        enum SliceMode
        {
            CellCount,
            CellSize
        }

        TileSet _tileset;

        OpenFileDialog FileDialog;
        private SliceMode _sliceMode;
        private string _workingFile;

        private string WorkingFile
        {
            get => _workingFile;
            set => Set(ref _workingFile, value);
        }

        private string PrevWorkingFile { get; set; }
        private int _sizeExp;
        private BitmapSource _tilesetImageSource;


        public ICommand BrowseCommand { get; private set; }

        public Canvas Canvas { get; set; }

        #region UI property bindings

        public BitmapSource TilesetImageSource
        {
            get => _tilesetImageSource;
            private set => Set(ref _tilesetImageSource, value);
        }

        public string[] SliceModeChoices => Enum.GetNames(typeof(SliceMode));

        public int SelectedSliceMode
        {
            get => (int) _sliceMode;
            set => _sliceMode = (SliceMode) value;
        }

        public int SizeExp
        {
            get => _sizeExp;
            set
            {
                Set(ref _sizeExp, value);
                Dimension = (int) Math.Pow(2, SizeExp);
            }
        }

        public int Dimension {
            get => _tileset.Dimension;
            set {
                _tileset.Dimension = value;
                RaisePropertyChanged(nameof(Dimension));
            }
        }

    #endregion

        public TilesetEditorViewModel () {
            _sizeExp = 5;
            const int dimension = 128;
            _tileset = new TileSet("New TilesetImageSource", dimension);

            FileDialog = new OpenFileDialog();
            FileDialog.Filter = "Image File|*.png;*.jpg";

            BrowseCommand = new RelayCommand(StartBrowse);
            
        }

        private void StartBrowse () {
            if (FileDialog.ShowDialog() != true) return;
            WorkingFile = FileDialog.FileName;
            UpdateImageSource();
        }

        private void UpdateImageSource () {

            // Review: We should probably be more specific with the naming.
            var tilesetFromFile = BitmapService.Instance.GetBitmapSource(WorkingFile);
            var nx = Math.Log(tilesetFromFile.PixelHeight, 2);
            var hd = (int) nx;
            var ny = Math.Log(tilesetFromFile.PixelWidth, 2);
            var wd = (int) ny;

            if (hd != nx || wd != ny) {
                WorkingFile = PrevWorkingFile;
                return;
            }

            PrevWorkingFile = WorkingFile;
            TilesetImageSource = tilesetFromFile;
        } 

    }

}
