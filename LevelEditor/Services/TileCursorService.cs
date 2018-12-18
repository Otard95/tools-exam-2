using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LevelEditor.Models;

namespace LevelEditor.Services
{
    public static class TileCursorService
    {
        public static Rectangle DefaultCursor => new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
            IsHitTestVisible = false
        };

        public static Rectangle DefaultMark = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0xFF)),
            IsHitTestVisible = false
        };

    /// <summary>
    /// Functional method that updates Tile Coordinate or returns the old one based on a mouse move.
    /// We keep old memory reference if mouse did not move.
    /// </summary>
    /// <param name="lastMouseCoordinate"></param>
    /// <param name="dimension"></param>
    /// <param name="e"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static TileCoordinate UpdateCursorCoordinate(TileCoordinate lastMouseCoordinate, int dimension, MouseEventArgs e, Canvas sender)
        {
            var position = e.GetPosition(sender);
            var newMouseCoordinate = new TileCoordinate(
                x: (int)(position.X / dimension),
                y: (int)(position.Y / dimension),
                tileSetMapId: 0,
                tileId: 0
            );

            return newMouseCoordinate;
        }

        public static bool IsNewTileCoordinate(TileCoordinate newMouseCoordinate, TileCoordinate lastMouseCoordinate) {
            return newMouseCoordinate != lastMouseCoordinate;
        }
    }
}
