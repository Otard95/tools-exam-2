using LevelEditor.ViewModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        public CanvasView()
        {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
            GenerateTiles();
            //Render();
        }

        private void CanvasElement_MouseDown (object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as Canvas);
            var dimension = ViewModel.Map.Dimension;
            var x = (int)(position.X / dimension);
            var y = (int)(position.Y / dimension);
            var tileSet = ViewModel.Map.TileSets.First();
            var tileKey = tileSet.TileKeys.First();
            ViewModel.Map.PlaceTile(x, y, tileSet, tileKey);
            Render();
        }

        private void GenerateTiles()
        {
            //var map = TileMapService.Instance.LoadMap("./TileMaps/DefaultTilemap");
            //ViewModel.Map = new TileMap(128);
            var tileSetToUse = TileService.Instance.GetTileset("./TileSets/DefaultTileset");
            var tileToPlace = tileSetToUse.TileKeys.First();
            ViewModel.Map.PlaceTile(0, 0, tileSetToUse, tileToPlace);
            ViewModel.Map.PlaceTile(1, 1, tileSetToUse, tileToPlace);
            ViewModel.Map.PlaceTile(2, 2, tileSetToUse, tileToPlace);
            ViewModel.Map.PlaceTile(3, 3, tileSetToUse, tileToPlace);
            Render();

        }

        private void Render()
        {
            CanvasElement.Children.Clear();
            foreach (var coordinate in ViewModel.Map.CoordinateMap) {
                var tileMapping = ViewModel.Map.GetTileMapping(coordinate.X, coordinate.Y);
                var tileSet = ViewModel.Map.GetTileSetFromMapping(tileMapping);
                var tileKey = ViewModel.Map.GetTileKey(tileMapping, tileSet);
                var tileSource = BitmapService.Instance.GetBitmapSource(tileKey.ContentPath);
                var tile = new Image {
                    Height = tileSet.Dimension,
                    Width = tileSet.Dimension,
                    Source = tileSource
                };
                Canvas.SetTop(tile, tileSet.Dimension * coordinate.Y);
                Canvas.SetLeft(tile, tileSet.Dimension * coordinate.X);
                CanvasElement.Children.Add(tile);
            }
        }

        private void CanvasElement_MouseMove(object sender, MouseEventArgs e) {

            var dimension = ViewModel.Map.Dimension;
            var position = e.GetPosition(sender as Canvas);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension)
            );

            if (!IsNewTileCoordinate(newMouseCoordinate)) return;
            ViewModel.LastMouseCoordinate = newMouseCoordinate;

            _mark.Height = dimension;
            _mark.Width = dimension;
            Render();
            Canvas.SetTop(_mark, dimension * newMouseCoordinate.Y);
            Canvas.SetLeft(_mark, dimension * newMouseCoordinate.X);
            CanvasElement.Children.Add(_mark);
        }

        private bool IsNewTileCoordinate(TileCoordinate newMouseCoordinate) {
            return newMouseCoordinate != ViewModel.LastMouseCoordinate;
        }
    }
}
