using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CommentCleanerWPF.Converters
{
    public class BoolToVisibleConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            bool val = (bool)value;
            return val ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            Visibility val = ( Visibility )value;
            return val == Visibility.Visible;
        }
    }
}
