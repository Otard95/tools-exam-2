using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : INotifyPropertyChanged {

        OpenFileDialog FileDialog;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BrowseCommand { get; private set; }

        public Canvas Canvas { get; set; }
        public BitmapSource Tileset { get; private set; }
        public string WorkingFile { get; set; }

        public TilesetEditorViewModel () {
            FileDialog = new OpenFileDialog();
            FileDialog.Filter = "Image File|*.png;*.jpg";

            BrowseCommand = new RelayCommand(StartBrowse);
            
            WorkingFile = "Images/NoTilesetImage.png";
            UpdateImageSource();

        }

        private void StartBrowse () {
            if (FileDialog.ShowDialog() == true) {
                WorkingFile = FileDialog.FileName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WorkingFile"));
                UpdateImageSource();
            }
        }

        private void UpdateImageSource () {
            Tileset = BitmapService.Instance.GetBitmapSource(WorkingFile);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tileset"));

        } 

    }

}
