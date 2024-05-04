using System.Globalization;
using System.Windows.Data;

namespace UserInterface
{
    public class IntervalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] interval = (double[]) value;
            return interval[0] + ";" + interval[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String input = (String)value;
            var values = input.Split(';');
            double[] interval = { double.Parse(values[0]), double.Parse(values[1]) };
            return interval;
        }
    }
}
