using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : INotifyPropertyChanged {
        
        private enum SliceMode {
            CellCount,
            CellSize
        }
        
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
        public int SizeExp { get => _sizeExp; set { _sizeExp = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dimention))); } }
        public int Dimention { get => (int) Math.Pow(2, SizeExp); }

        #endregion

        public TilesetEditorViewModel () {
            FileDialog = new OpenFileDialog();
            FileDialog.Filter = "Image File|*.png;*.jpg";

            BrowseCommand = new RelayCommand(StartBrowse);

            _sizeExp = 5;
            WorkingFile = "Images/NoTilesetImage.png";
            UpdateImageSource();

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
