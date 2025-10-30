using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMauiApp.Models;

namespace TodoMauiApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PriorityLevel.High => Colors.Red,
                PriorityLevel.Medium => Colors.Orange,
                PriorityLevel.Low => Colors.Gray,
                _ => Colors.Gray
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
