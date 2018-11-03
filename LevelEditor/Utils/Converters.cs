using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LevelEditor.Util {
    public static class Converters {
        
        public static BitmapSource BitmapToBitmapSource (Bitmap source) {
            return Imaging.CreateBitmapSourceFromHBitmap(
                            source.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
        }

        public static List<string> Enum2StringList<EnumType> (List<EnumType> list) {
            List<string> output = new List<string>();
            foreach (EnumType e in list) {
                output.Add(e.ToString());
            }
            return output;
        }

    }
}
