using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace AutomationControls.Converters
{
    [ValueConversion(typeof(List<byte>), typeof(String))]
    class ByteListToStringConverter : MarkupExtension, IValueConverter
    {

        public ByteListToStringConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            return Encoding.ASCII.GetString(((List<byte>)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return new List<byte>();
            return Encoding.ASCII.GetBytes(((string)value)).ToList();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }



}
