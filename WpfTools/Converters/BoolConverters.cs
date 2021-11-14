using System;
using System.Globalization;
using System.Windows;

namespace WpfTools
{
    public class BoolToObjectConverter : ValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register("TrueValue", typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register("FalseValue", typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

        public object TrueValue
        {
            get { return this.GetValue(BoolToObjectConverter.TrueValueProperty); }
            set { this.SetValue(BoolToObjectConverter.TrueValueProperty, value); }
        }

        public object FalseValue
        {
            get { return this.GetValue(BoolToObjectConverter.FalseValueProperty); }
            set { this.SetValue(BoolToObjectConverter.FalseValueProperty, value); }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? this.TrueValue : this.FalseValue;
            }

            throw new InvalidOperationException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (object.Equals(value, this.TrueValue))
            {
                return true;
            }

            if (object.Equals(value, this.FalseValue))
            {
                return false;
            }

            throw new InvalidOperationException();
        }
    }

    public sealed class BoolToVisibleConverter : BoolToObjectConverter
    {
        public static BoolToVisibleConverter Instance { get; } = new BoolToVisibleConverter();

        public BoolToVisibleConverter()
        {
            this.TrueValue = Visibility.Visible;
            this.FalseValue = Visibility.Collapsed;
        }
    }

    public sealed class BoolToCollapsedConverter : BoolToObjectConverter
    {
        public static BoolToCollapsedConverter Instance { get; } = new BoolToCollapsedConverter();

        public BoolToCollapsedConverter()
        {
            this.TrueValue = Visibility.Collapsed;
            this.FalseValue = Visibility.Visible;
        }
    }

    public sealed class BoolToInverseConverter : BoolToObjectConverter
    {
        public static BoolToInverseConverter Instance { get; } = new BoolToInverseConverter();

        public BoolToInverseConverter()
        {
            this.TrueValue = false;
            this.FalseValue = true;
        }
    }
}
