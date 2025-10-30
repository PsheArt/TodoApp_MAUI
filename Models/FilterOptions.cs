using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Models
{
    public enum SortOption
    {
        DueDate,
        Priority,
        Title
    }

    public class FilterOptions
    {
        public string? SearchText { get; set; }
        public bool ShowOverdueOnly { get; set; }
        public bool ShowNoDueDate { get; set; }
        public bool ShowHighPriority { get; set; }
        public SortOption SortBy { get; set; } = SortOption.DueDate;
    }
}
