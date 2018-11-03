using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LevelEditor.Services {
    public static class Converters {
        
        public static BitmapSource BitmapToBitmapSource (Bitmap source) {
            return Imaging.CreateBitmapSourceFromHBitmap(
                            source.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
        }

        // Michael: We should probably not use enums and string. This can be replaced by static classes with const strings or static strings. 
        public static List<string> Enum2StringList<EnumType> (List<EnumType> list) {
            List<string> output = new List<string>();
            foreach (EnumType e in list) {
                output.Add(e.ToString());
            }
            return output;
        }

    }
}
