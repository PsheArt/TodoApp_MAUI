using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Converters
{
    public class DueDateToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dueDate)
            {
                if (dueDate.Date == DateTime.Today && dueDate > DateTime.Now)
                    return Color.FromArgb("#FFF8E1"); 
                if (dueDate < DateTime.Now)
                    return Color.FromArgb("#FFEBEE"); 
            }
            return Colors.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
