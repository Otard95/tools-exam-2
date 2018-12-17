using System.ComponentModel;
using System.Diagnostics;
using LevelEditor.ViewModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LevelEditor.Models;
using LevelEditor.Services;
using Image = System.Windows.Controls.Image;

namespace LevelEditor.Views
{
    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : Page
    {

        public CanvasViewModel ViewModel => (CanvasViewModel) DataContext;

        private readonly Rectangle _mark = new Rectangle
        {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
            IsHitTestVisible = false
        };

        private readonly Rectangle _tileSetMark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00)),
            IsHitTestVisible = false
        };

        private readonly Rectangle _selectedTileSetTileMark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0xFF)),
            IsHitTestVisible = false
        };

        private System.Drawing.Rectangle _sliceRectangle = new System.Drawing.Rectangle(0,0,0,0);

        public CanvasView()
        {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
            ViewModel.TileSetCanvas = TileSetCanvas;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            var cache = new BitmapCache();
            TileSetCanvas.CacheMode = cache;
            CanvasElement.CacheMode = cache;
            Render();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.SelectedTileSet): OnNewTileSet();
                break;
                case nameof(ViewModel.SelectedTileId): RenderTileSet();
                break;
            }
        }

        private void OnNewTileSet()
        {
            ViewModel.SelectedTileId = 0;
            RenderTileSet();
        }

        private void CanvasElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as Canvas);
            var dimension = ViewModel.Map.Dimension;
            var x = (int) (position.X / dimension);
            var y = (int) (position.Y / dimension);
            ViewModel.EditMap(x, y);
            Render();
        }

        private void Render()
        {
            CanvasElement.Children.Clear();
            

            foreach (var coordinate in ViewModel.Map.CoordinateMap)
            {
                var tileMapping = ViewModel.Map.GetTileMapping(coordinate.X, coordinate.Y);
                var tileSet = ViewModel.Map.GetTileSetFromMapping(tileMapping);
                var tileKey = ViewModel.Map.GetTileKey(tileMapping, tileSet);
                var dimension = tileSet.Dimension;
                var x = tileSet.Dimension * coordinate.X;
                var y = tileSet.Dimension * coordinate.Y;
                _sliceRectangle.Width = dimension;
                _sliceRectangle.Height = dimension;
                _sliceRectangle.X = tileKey.X * dimension;
                _sliceRectangle.Y = tileKey.Y * dimension;
                
                var tileSource = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath, _sliceRectangle);
                var tile = new Image {
                    Height = dimension,
                    Width = dimension,
                    Source = tileSource
                };
                Canvas.SetTop(tile, y);
                Canvas.SetLeft(tile, x);
                CanvasElement.Children.Add(tile);
            }
        }

        private void CanvasElement_MouseMove(object sender, MouseEventArgs e)
        {

            var dimension = ViewModel.Map.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int) (position.X / dimension),
                y: (int) (position.Y / dimension)
            );

            if (!IsNewTileCoordinate(newMouseCoordinate, ViewModel.LastMouseCoordinate)) return;
            ViewModel.LastMouseCoordinate = newMouseCoordinate;

            _mark.Height = dimension;
            _mark.Width = dimension;
            Render();
            Canvas.SetTop(_mark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(_mark, dimension * newMouseCoordinate.X);
            CanvasElement.Children.Add(_mark);
        }

        private static bool IsNewTileCoordinate(TileCoordinate newMouseCoordinate, TileCoordinate lastMouseCoordinate)
        {
            return newMouseCoordinate != lastMouseCoordinate;
        }

        private void TileSetCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedTileSet == null)
                return;

            if (!(e.Source is Image img))
                return;
            var dimension = ViewModel.Map.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension)
            );
            _sliceRectangle.X = newMouseCoordinate.X * dimension;
            _sliceRectangle.Y = newMouseCoordinate.Y * dimension;
            _sliceRectangle.Width = dimension;
            _sliceRectangle.Height = dimension;

            var tileKey = ViewModel.SelectedTileSet.TileKeys
                .FirstOrDefault(tk =>
                    tk.ContentPath == ViewModel.SelectedTileSet.ContentPath && tk.X == newMouseCoordinate.X && tk.Y == newMouseCoordinate.Y);
            //var tileKey = ViewModel.SelectedTileSet.TileKeys.FirstOrDefault(tk =>
            //    BitmapService.Instance.GetBitmapSource(tk.ContentPath, _sliceRectangle) == img.Source);
            if (tileKey == null)
                return;
            ViewModel.SelectedTileSetTilePosition = ViewModel.LastTileSetMouseCoordinate;
            ViewModel.SelectedTileId = tileKey.Id;
        }

        private void TileSetCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.SelectedTileSet == null)
                return;

            var dimension = ViewModel.Map.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension)
            );


            if (!IsNewTileCoordinate(newMouseCoordinate, ViewModel.LastTileSetMouseCoordinate)) return;
            ViewModel.LastTileSetMouseCoordinate = newMouseCoordinate;

            var tileSetMark = _tileSetMark;
            tileSetMark.Height = dimension;
            tileSetMark.Width = dimension;
            Canvas.SetTop(tileSetMark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(tileSetMark, dimension * newMouseCoordinate.X);
            if(TileSetCanvas.Children.Contains(_tileSetMark))
                TileSetCanvas.Children.Remove(_tileSetMark);
            TileSetCanvas.Children.Add(tileSetMark);
        }

        private void RenderTileSet() {
            var tileSet = ViewModel.SelectedTileSet;
            if (tileSet == null)
                return;

            var dimension = tileSet.Dimension;
            TileSetCanvas.Children.Clear();
            var maxColumns = 256 / dimension;
            var column = 0;
            var row = 0;
            _sliceRectangle.Width = tileSet.Dimension;
            _sliceRectangle.Height = tileSet.Dimension;
            foreach (var tileKey in tileSet.TileKeys)
            {
                
                var y = column == maxColumns ? (++row) : row;
                var x = column == maxColumns ? (column = 0) : column;

                _sliceRectangle.X = tileKey.X * dimension;
                _sliceRectangle.Y = tileKey.Y * dimension;
                var tileSource = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath, _sliceRectangle);
                // var tile = BitmapService.Instance.GetImage(tileKey.ContentPath, dimension, SliceRectangle, tileSource);
                var tile = new Image {
                    Height = dimension,
                    Width = dimension,
                    Source = tileSource
                };

                var coordinate = new TileCoordinate(x, y);

                Canvas.SetTop(tile, dimension * coordinate.Y);
                Canvas.SetLeft(tile, dimension * coordinate.X);
                TileSetCanvas.Children.Add(tile);

                column++;
            }

            if (TileSetTileIsSelected()) {
                MarkTileSetTile(tileSet, dimension);
            }
        }

        private void MarkTileSetTile(TileSet tileSet, int dimension) {
            var tileSetSelectionMark = _selectedTileSetTileMark;
            tileSetSelectionMark.Height = tileSet.Dimension;
            tileSetSelectionMark.Width = tileSet.Dimension;
            Canvas.SetTop(tileSetSelectionMark, dimension * ViewModel.SelectedTileSetTilePosition.Y);
            Canvas.SetLeft(tileSetSelectionMark, dimension * ViewModel.SelectedTileSetTilePosition.X);
            TileSetCanvas.Children.Add(tileSetSelectionMark);
        }

        private bool TileSetTileIsSelected() {
            return ViewModel.SelectedTileId > 0;
        }
    }
}
