using LevelEditor.ViewModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
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

            var tileSet = TileService.Instance.GetTileset("./TileSets/DefaultTileset");
            var tileSource = tileSet[tileSet.TileKeys.First()];

            var tile = new Image
            {
                Height = tileSet.Dimension,
                Width = tileSet.Dimension,
                Source = tileSource
            };

            var tile2 = new Image {
                Height = tileSet.Dimension,
                Width = tileSet.Dimension,
                Source = tileSource
            };

            Canvas.SetTop(tile, 128);
            Canvas.SetLeft(tile, 128);
            Canvas.SetTop(tile2, 256);
            Canvas.SetLeft(tile2, 256);
            CanvasElement.Children.Add(tile);
            CanvasElement.Children.Add(tile2);

        }

    }
}
