
namespace CanvasTesting.ViewModel {

    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Drawing;
    using CanvasTesting.Model;
    using CanvasTesting.Commands;
    using CanvasTesting.Util;
    using System.Windows.Media.Imaging;

    public class CanvasViewModel : INotifyPropertyChanged {

        CanvasModel _canvas;
        Random _rn;
        Microsoft.Win32.OpenFileDialog _file_dialog;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RandomizeBackground { get; private set; }
        public ICommand DrawRectCommand { get; private set; }
        public ICommand OpenFileDialogCommand { get; private set; }
        public ICommand DrawImageCommand { get; private set; }

        public int SelectedX { get; set; }
        public int SelectedY { get; set; }
        public int SelectedW { get; set; }
        public int SelectedH { get; set; }
        public string FileSelect { get; set; }

        public CanvasViewModel(Canvas CanvasEl) {

            _canvas = new CanvasModel(CanvasEl);
            _rn = new Random();
            _file_dialog = new Microsoft.Win32.OpenFileDialog();
            _file_dialog.Filter = "Image File|*.png;*.jpg";

            RandomizeBackground = new RelayCommand(SetRandomBackground, () => true);
            DrawRectCommand = new RelayCommand(DrawRect, () => true);
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog, () => true);
            DrawImageCommand = new RelayCommand(DrawBitmap, () => !String.IsNullOrEmpty(FileSelect));

            SetRandomBackground();

        }

        private void OpenFileDialog () {

            if (_file_dialog.ShowDialog() == true) {
                FileSelect = _file_dialog.FileName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileSelect"));
            }
            
        }
        
        private void SetRandomBackground () {

            System.Windows.Media.Color randomColor = System.Windows.Media.Color.FromRgb((byte)_rn.Next(256), (byte)_rn.Next(256), (byte)_rn.Next(256));
            _canvas.Canvas.Background = new SolidColorBrush(randomColor);

        }

        private void DrawRect() {

            System.Windows.Media.Color randomColor = System.Windows.Media.Color.FromRgb((byte)_rn.Next(256), (byte)_rn.Next(256), (byte)_rn.Next(256));
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Width = SelectedW;
            rect.Height = SelectedH;
            Canvas.SetTop(rect, SelectedY);
            Canvas.SetLeft(rect, SelectedX);
            rect.Fill = new SolidColorBrush(randomColor);

            _canvas.Canvas.Children.Add(rect);

        }

        private void DrawBitmap () {

            Bitmap bmp = new Bitmap(FileSelect);
            var source = Converters.BitmapToBitmapSource(bmp);

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Width = bmp.Width;
            image.Height = bmp.Height;
            image.Source = source;
            Canvas.SetLeft(image, SelectedX);
            Canvas.SetTop(image, SelectedY);
            
            _canvas.Canvas.Children.Add(image);

        }

    }
}
