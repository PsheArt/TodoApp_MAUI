using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMauiApp.Models;

namespace TodoMauiApp.Helper
{
    public static class AppCategories
    {
        public static readonly List<Category> All = new()
        {
            new() { Name = "Все", Color = Colors.Transparent },
            new() { Name = "Работа", Color = Colors.OrangeRed },
            new() { Name = "Дом", Color = Colors.LightGreen },
            new() { Name = "Учёба", Color = Colors.CornflowerBlue },
            new() { Name = "Личное", Color = Colors.Plum }
        };
    }
}
