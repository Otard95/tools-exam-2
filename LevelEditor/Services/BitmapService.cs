using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LevelEditor.Domain.Exceptions;
using LevelEditor.Models;
using Newtonsoft.Json;
using Image = System.Windows.Controls.Image;

namespace LevelEditor.Services {
    public class BitmapService {
        private static BitmapService _instance;

        public static BitmapService Instance => _instance ?? (_instance = new BitmapService());

        private Dictionary<SliceKey, BitmapSource> BitmapSourceFactory { get; set; }
        private Dictionary<SliceKey, Image> ImageFactory { get; set; }


        private BitmapService()
        {
            BitmapSourceFactory = new Dictionary<SliceKey, BitmapSource>();
            ImageFactory = new Dictionary<SliceKey, Image>();
        }

        private static BitmapSource BitmapSourceFromPath(string path, Int32Rect? rect = null)
        {
            var image = new BitmapImage();
            using (var stream = new FileStream(path, FileMode.Open)) {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
            }

            return image;
        }

        public BitmapSource GetBitmapSource(string bitmapSourcePath, Rectangle? area = null) {
            try
            {
                if (string.IsNullOrEmpty(bitmapSourcePath))
                    return null;
                var key = new SliceKey(bitmapSourcePath, area);
                if (BitmapSourceFactory.TryGetValue(key, out var croppedBitmapSource))
                    return croppedBitmapSource;

                if (area.HasValue)
                {
                    var nonCroppedkey = new SliceKey(bitmapSourcePath, null);
                    if (!BitmapSourceFactory.TryGetValue(nonCroppedkey, out var bitmapSource))
                    {
                        bitmapSource = BitmapSourceFromPath(bitmapSourcePath);
                        BitmapSourceFactory.Add(nonCroppedkey, bitmapSource);
                    }

                    var croppedBitMap = new CroppedBitmap(bitmapSource, new Int32Rect(area.Value.X, area.Value.Y, area.Value.Width, area.Value.Height));
                    BitmapSourceFactory.Add(key, croppedBitMap);
                    return croppedBitMap;

                }
                else
                {

                    var bitMapSource = BitmapSourceFromPath(bitmapSourcePath);
                    BitmapSourceFactory.Add(key, bitMapSource);
                    return bitMapSource;
                }
            }
            catch (Exception e) {
                throw new BitmapLoadException($"Could not load bitmap from path with rect {area.ToString()}: {bitmapSourcePath}", e);
            }
        }

        internal class SliceKey: IEquatable<SliceKey> {
            public string ContentPath { get; set; }
            public Rectangle? Rect { get; set; }

            public SliceKey(string contentPath, Rectangle? rect)
            {
                ContentPath = contentPath;
                Rect = rect;
            }

            public bool Equals(SliceKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(ContentPath, other.ContentPath) && Rect.Equals(other.Rect);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SliceKey) obj);
            }

            public override int GetHashCode()
            {
                return 
                    $"{ContentPath.GetHashCode().ToString()},{(Rect.HasValue ? Rect.Value.GetHashCode().ToString() : "NULL")}"
                        .GetHashCode();
            }
        }

        public Image GetImage(string tileKeyContentPath, int dimension, Rectangle? sliceRectangle, BitmapSource tileSource)
        {
            var key = new SliceKey(tileKeyContentPath, sliceRectangle);
            if (ImageFactory.TryGetValue(key, out var image))
                return image;

            var tile = new Image {
                Height = dimension,
                Width = dimension,
                Source = tileSource
            };

            ImageFactory.Add(key, tile);
            return tile;

        }
    }


}
