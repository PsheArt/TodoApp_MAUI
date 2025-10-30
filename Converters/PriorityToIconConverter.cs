using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMauiApp.Models;

namespace TodoMauiApp.Converters
{
    public class PriorityToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not PriorityLevel priority)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ PriorityToIconConverter получил неожиданный тип: {value?.GetType()}");
                return "ⓘ"; 
            }

            return priority switch
            {
                PriorityLevel.High => "❗",
                PriorityLevel.Medium => "⚠️",
                PriorityLevel.Low => "ⓘ",
                _ => "ⓘ"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
