using System.Collections.Generic;
using System.Diagnostics;
using LevelEditor.ViewModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Windows.Controls;
using System.Windows.Input;
using LevelEditor.Models;
using LevelEditor.Services;
using Newtonsoft.Json;
using Image = System.Windows.Controls.Image;

namespace LevelEditor.Views
{
    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : Page
    {

        public CanvasViewModel ViewModel => (CanvasViewModel) DataContext;

        public CanvasView()
        {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
            GenerateTiles();
        }

        private void CanvasElement_MouseDown (object sender, MouseButtonEventArgs e) {
            // CanvasElement.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        }

        private void GenerateTiles()
        {
            //var map = TileMapService.Instance.LoadMap("./TileMaps/DefaultTilemap");

            var tileSetToUse = TileService.Instance.GetTileset("./TileSets/DefaultTileset");
            var map = new TileMap(128);
            var tileKeyToPlace = tileSetToUse.TileKeys.First();
            map.PlaceTile(0, 0, tileSetToUse, tileKeyToPlace);
            map.PlaceTile(1, 1, tileSetToUse, tileKeyToPlace);
            map.PlaceTile(2, 2, tileSetToUse, tileKeyToPlace);
            map.PlaceTile(3, 3, tileSetToUse, tileKeyToPlace);

            foreach (var coordinate in map.CoordinateMap)
            {
                var tileMapping = map.GetTileMapping(coordinate.X, coordinate.Y);
                var tileSet = map.GetTileSetFromMapping(tileMapping);
                var tileKey = map.GetTileKey(tileMapping, tileSet);
                var tileSource = tileSet[tileKey];
                var tile = new Image {
                    Height = tileSet.Dimension,
                    Width = tileSet.Dimension,
                    Source = tileSource
                };
                Canvas.SetTop(tile, 128*coordinate.Y);
                Canvas.SetLeft(tile, 128*coordinate.X);
                CanvasElement.Children.Add(tile);
            }
        }

    }
}
