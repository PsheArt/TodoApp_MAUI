using Android.Content;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoMauiApp.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class NotificationReceiver : BroadcastReceiver
    {
        private const string ChannelId = "todo_channel";

        public override void OnReceive(Context context, Intent intent)
        {
            var title = intent.GetStringExtra("title") ?? "Напоминание";
            var message = intent.GetStringExtra("message") ?? "";

            var notificationBuilder = new NotificationCompat.Builder(context, ChannelId)
                .SetSmallIcon(Resource.Drawable.notification_template_icon_low_bg) 
                .SetContentTitle(title)
                .SetContentText(message)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetAutoCancel(true);

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(intent?.ToString().GetHashCode() ?? 0, notificationBuilder.Build());
        }
    }
}
