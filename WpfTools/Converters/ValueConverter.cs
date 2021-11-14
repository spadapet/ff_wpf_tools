using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfTools
{
    public abstract class ValueConverter : DependencyObject, IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
