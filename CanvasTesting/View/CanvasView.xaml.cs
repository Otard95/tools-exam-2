using System;
using System.Windows.Controls;
using CanvasTesting.ViewModel;

namespace CanvasTesting.View {
    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : Page {
        public CanvasView () {
            InitializeComponent();
            DataContext = new CanvasViewModel(CanvasEl);
        }
    }
}
