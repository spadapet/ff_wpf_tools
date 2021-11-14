using System;
using System.Globalization;

namespace WpfTools
{
    public sealed class DelegateMultiValueConverter : MultiValueConverter
    {
        public delegate object ConvertFunc(object[] values, Type targetType, object parameter, CultureInfo culture);
        public delegate object[] ConvertBackFunc(object value, Type[] targetTypes, object parameter, CultureInfo culture);

        private readonly ConvertFunc convert;
        private readonly ConvertBackFunc convertBack;

        public DelegateMultiValueConverter(ConvertFunc convert, ConvertBackFunc convertBack = null)
        {
            this.convert = convert;
            this.convertBack = convertBack;
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.convert?.Invoke(values, targetType, parameter, culture) ?? throw new InvalidOperationException();
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.convertBack?.Invoke(value, targetTypes, parameter, culture) ?? throw new InvalidOperationException();
        }
    }
}
