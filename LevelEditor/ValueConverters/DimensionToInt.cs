using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LevelEditor.Domain;

namespace LevelEditor.ValueConverters
{
    public class DimensionToInt : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int i) || !TileDimensionRules.AllowedDimensions.Contains(i)) return 0;
            return TileDimensionRules.AllowedDimensions.ToList().IndexOf(i);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var i = System.Convert.ToInt32(value);
            if (i >= TileDimensionRules.NumOfDimensions || i < 0)
                return TileDimensionRules.AllowedDimensions[0];
            return TileDimensionRules.AllowedDimensions[i];
        }
    }
}
