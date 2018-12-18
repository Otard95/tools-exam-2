using LevelEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LevelEditor {
    /// <summary>
    /// Interaction logic for TilesetEditorView.xaml
    /// </summary>
    public partial class TilesetEditorWindow : Window {

        public TilesetEditorViewModel ViewModel => (TilesetEditorViewModel) DataContext;
        private readonly List<Line> _verticalLines = new List<Line>();
        private readonly List<Line> _horizontalLines = new List<Line>();
        private SolidColorBrush _lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public TilesetEditorWindow () {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnActivated (EventArgs e) {
            base.OnActivated(e);
            ViewModel_PropertyChanged(null, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }

        private void ViewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e) {

            if (TileSetIsNotLoaded()) return;

            ClearPreviousRender();

            var height = ViewModel.TileSetImageSource.PixelHeight;
            var width = ViewModel.TileSetImageSource.PixelWidth;
            var dimension = ViewModel.Dimension;

            var numHorizontalLines = (int)Math.Ceiling(height * 1d / ViewModel.Dimension);
            var numVerticalLines = (int)Math.Ceiling(width * 1d / ViewModel.Dimension);

            var tileSetImage = new Image {
                Source = ViewModel.TileSetImageSource,
                Height = height,
                Width = width,
            };
            CanvasElement.Children.Add(tileSetImage);

            // TODO: Explain better what is going on here.
            for (var x = 0; x < numHorizontalLines; x++) {
                if (_horizontalLines.Count <= x) {
                    var lineToAdd = new Line {
                        X1 = 0,
                        X2 = width,
                        Y1 = x * dimension,
                        Y2 = x * dimension,
                        Stroke = _lineBrush,
                        StrokeThickness = 2,
                    };
                    _horizontalLines.Add(lineToAdd);
                    CanvasElement.Children.Add(lineToAdd);
                }
                else {
                    _horizontalLines[x].Y1 = x * dimension;
                    _horizontalLines[x].Y2 = x * dimension;
                    CanvasElement.Children.Add(_horizontalLines[x]);
                }
            }
            for (var y = 0; y < numVerticalLines; y++) {
                if (_verticalLines.Count <= y) {
                    var lineToAdd = new Line {
                        X1 = y * dimension,
                        X2 = y * dimension,
                        Y1 = 0,
                        Y2 = height,
                        Stroke = _lineBrush,
                        StrokeThickness = 2,
                    };
                    _verticalLines.Add(lineToAdd);
                    CanvasElement.Children.Add(lineToAdd);
                }
                else {
                    _verticalLines[y].X1 = y * dimension;
                    _verticalLines[y].X2 = y * dimension;
                    CanvasElement.Children.Add(_verticalLines[y]);
                }
            }

        }

        private void ClearPreviousRender() {
            _verticalLines.Clear();
            _horizontalLines.Clear();
            CanvasElement.Children.Clear();
        }

        private bool TileSetIsNotLoaded() {
            return string.IsNullOrEmpty(ViewModel.WorkingFile);
        }
    }
}
