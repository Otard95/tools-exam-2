using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        private Dictionary<string, Bitmap> BitmapFactory { get; set; }
        private Dictionary<SliceKey, BitmapSource> BitmapSourceFactory { get; set; }

        private BitmapService()
        {
            BitmapFactory = new Dictionary<string, Bitmap>();
            BitmapSourceFactory = new Dictionary<SliceKey, BitmapSource>();
        }

        ~BitmapService()
        {
            foreach (var bitmapFactoryValue in BitmapFactory.Values)
            {
                bitmapFactoryValue.Dispose();
            }
        }

        private static BitmapSource BitmapToBitmapSource(Bitmap source) {
            return Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public BitmapSource GetBitmapSource(string bitmapSourcePath, Rectangle? area = null) {
            try {
                var key = new SliceKey(bitmapSourcePath, area);
                if (BitmapSourceFactory.TryGetValue(key, out var croppedBitmapSource))
                    return croppedBitmapSource;
                if (!BitmapFactory.TryGetValue(bitmapSourcePath, out var bitMap))
                    BitmapFactory.Add(bitmapSourcePath, (bitMap = new Bitmap(bitmapSourcePath)));

                if (area.HasValue)
                {
                    var slicedBitmapPart = bitMap.Clone(area.Value, PixelFormat.DontCare);
                    var bitMapSource = BitmapToBitmapSource(slicedBitmapPart);
                    BitmapSourceFactory.Add(key, bitMapSource);
                    return bitMapSource;
                }
                else
                {
                    var bitMapSource = BitmapToBitmapSource(bitMap);
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
                unchecked
                {
                    return ((ContentPath != null ? ContentPath.GetHashCode() : 0) * 397) ^ Rect.GetHashCode();
                }
            }
        }
    }


}
