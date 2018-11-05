﻿using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
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
        SliceMode _slice_mode;
        private string WorkingFile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public ICommand BrowseCommand { get; private set; }
        
        public Canvas Canvas { get; set; }

        #region UI proportie bindings

        public BitmapSource Tileset { get; private set; }
        public string[] SliceModeChoices { get { return Enum.GetNames(typeof(SliceMode)); } }
        public int SelectedSliceMode { get => (int) _slice_mode; set => _slice_mode = (SliceMode) value; }
        public int Dimention { get; set; }

        #endregion

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
