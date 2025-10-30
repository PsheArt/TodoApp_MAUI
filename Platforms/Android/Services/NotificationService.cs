using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMauiApp.Services;
using Application = Android.App.Application;

namespace TodoMauiApp.Platforms.Android.Services
{
    public class NotificationService : INotificationService
    {
        private const string ChannelId = "todo_channel";
        private const string ChannelName = "To-Do Reminders";
        private const int NotificationIdBase = 1000;

        public async Task ScheduleNotificationAsync(string title, string message, DateTime triggerTime, string taskId)
        {
            if (triggerTime <= DateTime.Now) return;

            // Создаём канал (обязательно для Android 8+)
            CreateNotificationChannel();

            // PendingIntent для AlarmManager
            var intent = new Intent(Application.Context, typeof(NotificationReceiver));
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);
            intent.PutExtra("taskId", taskId);

            var pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                taskId.GetHashCode(),
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            );

            // Запланировать срабатывание
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            var triggerMillis = (long)(triggerTime - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerMillis, pendingIntent);
            }
            else
            {
                alarmManager?.SetExact(AlarmType.RtcWakeup, triggerMillis, pendingIntent);
            }
        }

        private void CreateNotificationChannel()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default);
                var notificationManager = (NotificationManager?)Application.Context.GetSystemService(Context.NotificationService);
                notificationManager?.CreateNotificationChannel(channel);
            }
        }
    }
}
