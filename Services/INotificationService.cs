using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Services
{
    public interface INotificationService
    {
        Task ScheduleNotificationAsync(string title, string message, DateTime triggerTime, string taskId);
    }
}
