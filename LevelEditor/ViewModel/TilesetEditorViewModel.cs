﻿using GalaSoft.MvvmLight.CommandWpf;
using LevelEditor.Models;
using LevelEditor.Services;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using LevelEditor.Domain;

namespace LevelEditor.ViewModel {
    public class TilesetEditorViewModel : ViewModelBase
    {
        private readonly TileSet _tileSet;
        private readonly OpenFileDialog _fileDialog;
        private SliceType _sliceType;
        private string _workingFile;
        private string PrevWorkingFile { get; set; }
        private int _sizeExp;

        public ICommand BrowseCommand { get; private set; }

        public Canvas Canvas { get; set; }

        #region UI property bindings

        public BitmapSource TileSetImageSource => BitmapService.Instance.GetBitmapSource(WorkingFile);

        public string WorkingFile {
            get => _workingFile;
            private set
            {
                Set(ref _workingFile, value);
                RaisePropertyChanged(nameof(TileSetImageSource));
            }
        }


        public string[] SliceModeChoices => Enum.GetNames(typeof(SliceType));

        public int SelectedSliceMode
        {
            get => (int) _sliceType;
            set => _sliceType = (SliceType) value;
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
            get => _tileSet.Dimension;
            set {
                _tileSet.Dimension = value;
                RaisePropertyChanged(nameof(Dimension));
            }
        }

    #endregion

        public TilesetEditorViewModel () {
            _sizeExp = 5;
            const int dimension = 128;
            _tileSet = new TileSet(Guid.Empty, "New TileSetImageSource", dimension);


            _fileDialog = new OpenFileDialog {Filter = $"Image File|*.{FileExtension.Png};*.{FileExtension.Jpg}"};

            BrowseCommand = new RelayCommand(StartBrowse);
            
        }

        private void StartBrowse () {
            if (_fileDialog.ShowDialog() != true) return;
            WorkingFile = _fileDialog.FileName;
            // UpdateImageSource();
        }

        private void UpdateImageSource () {

            // Review: We should probably be more specific with the naming.
            //var tilesetFromFile = BitmapService.Instance.GetBitmapSource(WorkingFile);
            //var nx = Math.Log(tilesetFromFile.PixelHeight, 2);
            //var hd = (int)nx;
            //var ny = Math.Log(tilesetFromFile.PixelWidth, 2);
            //var wd = (int)ny;

            //if (hd != nx || wd != ny) {
            //    WorkingFile = PrevWorkingFile;
            //    return;
            //}

            //PrevWorkingFile = WorkingFile;
        } 

    }

}
