using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Services
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        void SetTheme(bool isDark);
        event Action? ThemeChanged;
    }
}
