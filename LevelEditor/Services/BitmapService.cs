using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using LevelEditor.Domain.Exceptions;
using LevelEditor.Models;
using Newtonsoft.Json;

namespace LevelEditor.Services {
    public class BitmapService {
        private static BitmapService _instance;

        public static BitmapService Instance => _instance ?? (_instance = new BitmapService());

        public Dictionary<string, BitmapSource> BitmapFactory { get; set; }

        private BitmapService()
        {
            BitmapFactory = new Dictionary<string, BitmapSource>();
        }

        private static BitmapSource BitmapToBitmapSource(Bitmap source) {
            return Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public BitmapSource GetBitmapSource(string bitmapSourcePath)
        {
            if (BitmapFactory.TryGetValue(bitmapSourcePath, out var bitMapSource))
                return bitMapSource;

            try
            {
                var bitmap = new Bitmap(bitmapSourcePath);
                var bitmapSource = BitmapToBitmapSource(bitmap);
                BitmapFactory.Add(bitmapSourcePath, bitmapSource);
                return bitmapSource;
            }
            catch (Exception e)
            {
                throw new BitmapLoadException($"Could not load bitmap from path: {bitmapSourcePath}", e);
            }
        }
    }
}
