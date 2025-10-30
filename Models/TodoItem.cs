using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Models
{
    public class TodoItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Category { get; set; } = "Все";
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public DateTime? DueDate { get; set; }
        public List<SubTask> SubTasks { get; set; } = new();
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public int RecurrenceInterval { get; set; } = 1; // для CustomDays
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }
    public enum RecurrenceType
    {
        None,
        Daily,
        Weekly,
        Monthly,
        CustomDays
    }

}
