
using System.Windows.Controls;

namespace CanvasTesting.Model {
    public class CanvasModel {

        Canvas _canvas;

        public CanvasModel (Canvas canvas) {
            _canvas = canvas;
        }

        public Canvas Canvas { get { return _canvas; } private set { _canvas = value; } }

    }
}
