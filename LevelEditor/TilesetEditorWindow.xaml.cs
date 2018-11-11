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
        private List<Line> _verticalLines = new List<Line>();
        private List<Line> _horizontalLines = new List<Line>();
        private SolidColorBrush _lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public TilesetEditorWindow () {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e) {

            if (string.IsNullOrEmpty(ViewModel.WorkingFile)) return;

            int height = ViewModel.TileSetImageSource.PixelHeight;
            int width = ViewModel.TileSetImageSource.PixelWidth;
            int dimention = ViewModel.Dimension;

            int numHorizontalLines = (int) Math.Ceiling(height * 1f / ViewModel.Dimension);
            int numVerticalLines = (int) Math.Ceiling(width * 1f / ViewModel.Dimension);

            CanvasElement.Children.Clear();

            Image tilesetImage = new Image {
                Source = ViewModel.TileSetImageSource,
            };
            CanvasElement.Children.Add(tilesetImage);
            
            for (int x = 0; x < numHorizontalLines; x++) {
                if (_horizontalLines.Count <= x) {
                    Line lineToAdd = new Line {
                        X1 = 0,
                        X2 = width,
                        Y1 = x * dimention,
                        Y2 = x * dimention,
                        Stroke = _lineBrush,
                        StrokeThickness = 2,
                    };
                    _horizontalLines.Add(lineToAdd);
                    CanvasElement.Children.Add(lineToAdd);
                } else {
                    _horizontalLines[x].Y1 = x * dimention;
                    _horizontalLines[x].Y2 = x * dimention;
                    CanvasElement.Children.Add(_horizontalLines[x]);
                }
            }
            for (int y = 0; y < numVerticalLines; y++) {
                if (_verticalLines.Count <= y) {
                    Line lineToAdd = new Line {
                        X1 = y * dimention,
                        X2 = y * dimention,
                        Y1 = 0,
                        Y2 = height,
                        Stroke = _lineBrush,
                        StrokeThickness = 2,
                    };
                    _verticalLines.Add(lineToAdd);
                    CanvasElement.Children.Add(lineToAdd);
                } else {
                    _verticalLines[y].X1 = y * dimention;
                    _verticalLines[y].X2 = y * dimention;
                    CanvasElement.Children.Add(_verticalLines[y]);
                }
            }

        }
    }
}
