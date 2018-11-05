using LevelEditor.ViewModel;
using System.Windows;

namespace LevelEditor {
    /// <summary>
    /// Interaction logic for TilesetEditorView.xaml
    /// </summary>
    public partial class TilesetEditorWindow : Window {

        public TilesetEditorViewModel ViewModel => (TilesetEditorViewModel) DataContext;

        public TilesetEditorWindow () {
            InitializeComponent();
            ViewModel.Canvas = CanvasElement;
        }
    }
}
