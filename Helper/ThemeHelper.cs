using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Helper
{
    public static class ThemeHelper
    {
        public static Color Background =>
            Application.Current?.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#1E1E1E")
                : Colors.White;

        public static Color Text =>
            Application.Current?.RequestedTheme == AppTheme.Dark
                ? Colors.White
                : Colors.Black;

        public static Color SecondaryText =>
            Application.Current?.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#AAAAAA")
                : Colors.Gray;

        public static Color PrimaryButton =>
            Application.Current?.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#0A4A7A")
                : Color.FromArgb("#4A90E2");
    }
}
