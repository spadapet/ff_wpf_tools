using System;
using System.Globalization;
using System.Windows;

namespace WpfTools
{
    public class ObjectToObjectConverter : ValueConverter
    {
        public static readonly DependencyProperty NonNullValueProperty = DependencyProperty.Register("NonNullValue", typeof(object), typeof(ObjectToObjectConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register("NullValue", typeof(object), typeof(ObjectToObjectConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty EmptyStringValueProperty = DependencyProperty.Register("EmptyStringValue", typeof(object), typeof(ObjectToObjectConverter), new PropertyMetadata(null));

        public object NonNullValue
        {
            get { return this.GetValue(ObjectToObjectConverter.NonNullValueProperty); }
            set { this.SetValue(ObjectToObjectConverter.NonNullValueProperty, value); }
        }

        public object NullValue
        {
            get { return this.GetValue(ObjectToObjectConverter.NullValueProperty); }
            set { this.SetValue(ObjectToObjectConverter.NullValueProperty, value); }
        }

        public object EmptyStringValue
        {
            get { return this.GetValue(ObjectToObjectConverter.EmptyStringValueProperty); }
            set { this.SetValue(ObjectToObjectConverter.EmptyStringValueProperty, value); }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (this.ReadLocalValue(ObjectToObjectConverter.EmptyStringValueProperty) != DependencyProperty.UnsetValue && value is string valueString && string.IsNullOrEmpty(valueString))
                {
                    return this.EmptyStringValue;
                }

                return this.NonNullValue;
            }

            return this.NullValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }

    public class ObjectsEqualConverter : ValueConverter
    {
        public static readonly DependencyProperty EqualValueProperty = DependencyProperty.Register("EqualValue", typeof(object), typeof(ObjectsEqualConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty NotEqualValueProperty = DependencyProperty.Register("NotEqualValue", typeof(object), typeof(ObjectsEqualConverter), new PropertyMetadata(null));

        public object EqualValue
        {
            get { return this.GetValue(ObjectsEqualConverter.EqualValueProperty); }
            set { this.SetValue(ObjectsEqualConverter.EqualValueProperty, value); }
        }

        public object NotEqualValue
        {
            get { return this.GetValue(ObjectsEqualConverter.NotEqualValueProperty); }
            set { this.SetValue(ObjectsEqualConverter.NotEqualValueProperty, value); }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, parameter) ? this.EqualValue : this.NotEqualValue;
        }
    }
}
