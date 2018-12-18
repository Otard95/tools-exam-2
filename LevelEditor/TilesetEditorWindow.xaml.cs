using LevelEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LevelEditor.Models;
using LevelEditor.Services;

namespace LevelEditor {
    /// <summary>
    /// Interaction logic for TilesetEditorView.xaml
    /// </summary>
    public partial class TilesetEditorWindow : Window {

        public TilesetEditorViewModel ViewModel => (TilesetEditorViewModel) DataContext;
        private readonly Rectangle _mark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
            IsHitTestVisible = false
        };
        private List<Line> _verticalLines = new List<Line>();
        private List<Line> _horizontalLines = new List<Line>();
        private readonly SolidColorBrush _lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private System.Drawing.Rectangle _sliceRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
        private readonly Rectangle _tileMark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0xFF)),
            IsHitTestVisible = false
        };

        public TilesetEditorWindow () {
            InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnActivated (EventArgs e) {
            base.OnActivated(e);
            CanvasElement.Children.Clear();
            ViewModel_PropertyChanged(ViewModel, new System.ComponentModel.PropertyChangedEventArgs(nameof(ViewModel.TileSet)));
        }

        /// <summary>
        /// Override close to prevent exceptions when Garbage collector has not yet cleared a previous view's Canvas Element 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e) {
            CanvasElement.Children.Clear();
            base.OnClosed(e);
        }

        private void CanvasElement_MouseMove(object sender, MouseEventArgs e) {
            var dimension = ViewModel.Dimension;
            var newMouseCoordinate = TileCursorService.UpdateCursorCoordinate(ViewModel.LastMouseCoordinate, dimension, e, (sender as Canvas));

            if (TileCursorService.IsNewTileCoordinate(newMouseCoordinate, ViewModel.LastMouseCoordinate)) {
                ViewModel.LastMouseCoordinate = newMouseCoordinate;
                Render();
                RenderCursor(dimension, newMouseCoordinate);
            }
        }

        private void RenderTileMark(int dimension, TileCoordinate newMouseCoordinate)
        {
            if (_tileMark == null) return;
            var tileMark = _tileMark;
            tileMark.Height = dimension;
            tileMark.Width = dimension;
            Canvas.SetTop(tileMark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(tileMark, dimension * newMouseCoordinate.X);
            if (CanvasElement.Children.Contains(_tileMark))
                CanvasElement.Children.Remove(_tileMark);
            CanvasElement.Children.Add(tileMark);

        }

        private void RenderCursor(int dimension, TileCoordinate newMouseCoordinate)
        {
            _mark.Height = dimension;
            _mark.Width = dimension;
            RenderCursorElement(_mark, dimension, newMouseCoordinate);
        }

        private void RenderCursorElement(UIElement mark, int dimension, TileCoordinate newMouseCoordinate) {
            Canvas.SetTop(mark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(mark, dimension * newMouseCoordinate.X);

            if (CanvasElement.Children.Contains(mark))
            {
                CanvasElement.Children.Remove(mark);
            }
            CanvasElement.Children.Add(mark);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }

        private void ViewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName != nameof(ViewModel.TileSet) && e.PropertyName != nameof(ViewModel.Dimension)) return;

            Render();
        }

        private void Render() {
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

            _horizontalLines = GenerateHorizontalLines(width, dimension, numHorizontalLines, _horizontalLines).ToList();
            _horizontalLines.ForEach(line => CanvasElement.Children.Add(line));
            _verticalLines = GenerateVerticalLines(height, dimension, numVerticalLines, _verticalLines).ToList();
            _verticalLines.ForEach(line => CanvasElement.Children.Add(line));

            if (TileSetTileIsSelected()) {
                MarkTileSetTile(ViewModel.TileSet, dimension);
            }
        }

        private void MarkTileSetTile(TileSet tileSet, int dimension) {
            var tileSetSelectionMark = _tileMark;
            tileSetSelectionMark.Height = tileSet.Dimension;
            tileSetSelectionMark.Width = tileSet.Dimension;
            Canvas.SetTop(tileSetSelectionMark, dimension * ViewModel.SelectedTilePosition.Y);
            Canvas.SetLeft(tileSetSelectionMark, dimension * ViewModel.SelectedTilePosition.X);
            if (CanvasElement.Children.Contains(tileSetSelectionMark))
            {
                CanvasElement.Children.Remove(tileSetSelectionMark);
            }

            CanvasElement.Children.Add(tileSetSelectionMark);
        }

        private bool TileSetTileIsSelected() {
            return ViewModel.SelectedTileId > 0;
        }


        private IEnumerable<Line> GenerateVerticalLines(int height, int dimension, int numVerticalLines, List<Line> oldLines) {
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
                    yield return lineToAdd;
                }
                else {
                    var oldLine = oldLines[y];
                    oldLine.X1 = y * dimension;
                    oldLine.X2 = y * dimension;
                    yield return oldLine;
                }
            }
        }

        private IEnumerable<Line> GenerateHorizontalLines(int width, int dimension, int numHorizontalLines, List<Line> oldLines) {
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
                    yield return lineToAdd;
                }
                else
                {
                    var oldLine = oldLines[x];
                    oldLine.Y1 = x * dimension;
                    oldLine.Y2 = x * dimension;
                    yield return oldLine;
                }
            }
        }

        private void ClearPreviousRender() {
            _verticalLines.Clear();
            _horizontalLines.Clear();
            CanvasElement.Children.Clear();
        }

        private bool TileSetIsNotLoaded() {
            return ViewModel.TileSetImageSource == null;
        }

        private void CloseButton_Click (object sender, RoutedEventArgs e) {
            Close();
        }

        private void CanvasElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TileSetIsNotLoaded() || !(e.Source is Image img))
                return;

            var dimension = ViewModel.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension),
                tileSetMapId: 0,
                tileId: 0
            );
            _sliceRectangle.X = newMouseCoordinate.X * dimension;
            _sliceRectangle.Y = newMouseCoordinate.Y * dimension;
            _sliceRectangle.Width = dimension;
            _sliceRectangle.Height = dimension;

            var tileKey = ViewModel.TileSet.TileKeys
                .FirstOrDefault(tk =>
                    tk.ContentPath == ViewModel.TileSet.ContentPath && tk.X == newMouseCoordinate.X && tk.Y == newMouseCoordinate.Y);

            if (tileKey == null)
                return;

            ViewModel.SelectedTilePosition = ViewModel.LastMouseCoordinate;
            ViewModel.SelectedTileId = tileKey.Id;

            _sliceRectangle.X = tileKey.X * dimension;
            _sliceRectangle.Y = tileKey.Y * dimension;
            var source = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath, _sliceRectangle);

            ViewModel.SelectedTileImage = source;

            Render();
        }
    }
}
