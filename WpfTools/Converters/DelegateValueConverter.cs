using System;
using System.Globalization;

namespace WpfTools
{
    public sealed class DelegateValueConverter : ValueConverter
    {
        public delegate object ConvertFunc(object value, Type targetType, object parameter, CultureInfo culture);

        private readonly ConvertFunc convert;
        private readonly ConvertFunc convertBack;

        public DelegateValueConverter(ConvertFunc convert, ConvertFunc convertBack = null)
        {
            this.convert = convert;
            this.convertBack = convertBack;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.convert?.Invoke(value, targetType, parameter, culture) ?? throw new InvalidOperationException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.convertBack?.Invoke(value, targetType, parameter, culture) ?? throw new InvalidOperationException();
        }
    }
}
