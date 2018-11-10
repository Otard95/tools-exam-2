using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Models;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : INotifyPropertyChanged {
        
        enum SliceMode {
            CellCount,
            CellSize
        }

        TileSet _tileset;
        
        OpenFileDialog FileDialog;
        SliceMode _sliceMode;
        private string WorkingFile { get; set; }
        private string PrevWorkingFile { get; set; }
        int _sizeExp;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public ICommand BrowseCommand { get; private set; }
        
        public Canvas Canvas { get; set; }

        #region UI proportie bindings

        public BitmapSource Tileset { get; private set; }
        public string[] SliceModeChoices { get { return Enum.GetNames(typeof(SliceMode)); } }
        public int SelectedSliceMode { get => (int) _sliceMode; set => _sliceMode = (SliceMode) value; }
        public int SizeExp {
            get => _sizeExp;
            set {
                _sizeExp = value;
                _tileset.Dimension = (int) Math.Pow(2, SizeExp);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dimention)));
            }
    }
        public int Dimention { get => _tileset.Dimension; }

        #endregion

        public TilesetEditorViewModel () {
            _sizeExp = 5;
            _tileset = new TileSet("New Tileset", Dimention);

            FileDialog = new OpenFileDialog();
            FileDialog.Filter = "Image File|*.png;*.jpg";

            BrowseCommand = new RelayCommand(StartBrowse);
            
        }

        private void StartBrowse () {
            if (FileDialog.ShowDialog() == true) {
                WorkingFile = FileDialog.FileName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingFile)));
                UpdateImageSource();
            }
        }

        private void UpdateImageSource () {
            BitmapSource tilesetFromFile = BitmapService.Instance.GetBitmapSource(WorkingFile);
            double nx = Math.Log(tilesetFromFile.PixelHeight, 2);
            int hd = (int) nx;
            double ny = Math.Log(tilesetFromFile.PixelWidth, 2);
            int wd = (int) ny;

            if (hd != nx || wd != ny) {
                WorkingFile = PrevWorkingFile;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingFile)));
                return;
            }

            PrevWorkingFile = WorkingFile;
            Tileset = tilesetFromFile;
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tileset)));
        } 

    }

}
