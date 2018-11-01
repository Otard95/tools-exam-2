using LevelEditor.Factories;
using LevelEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void GenerateTiles() {

            TileFactory.Instance.LoadTileSet();

            Image tile = new Image();
            tile.Height = 128;
            tile.Width = 128;
            Canvas.SetTop(tile, 128);
            Canvas.SetLeft(tile, 128);

            CanvasElement.Children.Add(tile);

        }

    }
}
