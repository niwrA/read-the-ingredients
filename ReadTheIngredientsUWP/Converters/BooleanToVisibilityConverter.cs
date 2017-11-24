using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ReadTheIngredientsUWP.Converters
{
   public class BooleanToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var b = value as bool?;
            return b == null ? Visibility.Collapsed : (b.Value ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = value as Visibility?;
            return v == null ? (object)null : v.Value == Visibility.Visible;
        }
    }
}
