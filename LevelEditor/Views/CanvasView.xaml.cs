﻿using System.ComponentModel;
using System.Diagnostics;
using LevelEditor.ViewModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Windows;
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

        private readonly Rectangle _mark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
            IsHitTestVisible = false
        }; 

        private Image _tileMark;

        private readonly Rectangle _tileSetMark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0xFF)),
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
                var tileSet = ViewModel.Map.TileSetMap[coordinate.TileSetMapId];

                var tileKey = ViewModel.Map.GetTileKey(coordinate, tileSet);
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
            var newCoordinate = TileCursorService.UpdateCursorCoordinate(ViewModel.LastMouseCoordinate, dimension, e, (sender as Canvas));

            if (TileCursorService.IsNewTileCoordinate(newCoordinate, ViewModel.LastMouseCoordinate))
            {
                ViewModel.LastMouseCoordinate = newCoordinate;
                Render();
                RenderTileCursor(dimension, newCoordinate);
            }
        }

        private void RenderTileCursor(int dimension, TileCoordinate newMouseCoordinate) {
            _mark.Height = dimension;
            _mark.Width = dimension;
            RenderCursorElement(_mark, dimension, newMouseCoordinate);
            if (_tileMark != null) {
                RenderCursorElement(_tileMark, dimension, newMouseCoordinate);
            }
        }

        private void RenderCursorElement(UIElement mark, int dimension, TileCoordinate newMouseCoordinate) {
            Canvas.SetTop(mark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(mark, dimension * newMouseCoordinate.X);

            CanvasElement.Children.Add(mark);
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
                y: (int)(position.Y / dimension),
                tileSetMapId:0,
                tileId: 0
            );
            _sliceRectangle.X = newMouseCoordinate.X * dimension;
            _sliceRectangle.Y = newMouseCoordinate.Y * dimension;
            _sliceRectangle.Width = dimension;
            _sliceRectangle.Height = dimension;

            var tileKey = ViewModel.SelectedTileSet.TileKeys
                .FirstOrDefault(tk =>
                    tk.ContentPath == ViewModel.SelectedTileSet.ContentPath && tk.X == newMouseCoordinate.X && tk.Y == newMouseCoordinate.Y);

            if (tileKey == null)
                return;
            ViewModel.SelectedTileSetTilePosition = ViewModel.LastTileSetMouseCoordinate;
            ViewModel.SelectedTileId = tileKey.Id;

            _sliceRectangle.X = tileKey.X * dimension;
            _sliceRectangle.Y = tileKey.Y * dimension;
            var source = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath, _sliceRectangle);
            _tileMark = new Image {
                Height = dimension,
                Width = dimension,
                Source = source,
                Opacity = .3
            };
        }

        private void TileSetCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.SelectedTileSet == null)
                return;

            var dimension = ViewModel.Map.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension),
                tileSetMapId: 0,
                tileId: 0
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
            TileSetCanvas.Children.Clear();
            if (tileSet == null)
                return;

            var dimension = tileSet.Dimension;

            _sliceRectangle.Width = tileSet.Dimension;
            _sliceRectangle.Height = tileSet.Dimension;
            var maxWidth = 0;
            var maxHeight = 0;
            foreach (var tileKey in tileSet.TileKeys)
            {
                _sliceRectangle.X = tileKey.X * dimension;
                _sliceRectangle.Y = tileKey.Y * dimension;
                if(_sliceRectangle.X > maxWidth)
                {
                    maxWidth = _sliceRectangle.X;
                }
                if (_sliceRectangle.Y > maxHeight) {
                    maxHeight = _sliceRectangle.Y;
                }
                var tileSource = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath, _sliceRectangle);
                var tile = new Image {
                    Height = dimension,
                    Width = dimension,
                    Source = tileSource
                };

                Canvas.SetTop(tile, dimension * tileKey.Y);
                Canvas.SetLeft(tile, dimension * tileKey.X);
                TileSetCanvas.Children.Add(tile);
            }

            ViewModel.TileSetWidth = maxWidth + dimension;
            ViewModel.TileSetHeight = maxHeight + dimension;

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
