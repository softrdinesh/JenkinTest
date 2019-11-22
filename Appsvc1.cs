using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Media.Audiofx.Equalizer;

namespace Appsvc1
{

    [Service]
    public class Appsvc1 : Service
    {
        static readonly string TAG = "X:" + typeof(Appsvc1).Name;
        static readonly int TimerWait = 4000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;
        static readonly int NOTIFICATION_ID = 1000;
        static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";
        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(TAG, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }
            int unMber_unRead_sms = 1;

            var t = new Thread(() =>
            {
                while (true)
                {
                    NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("Sample Notification")
            .SetContentText("Hello World! This is my first notification!")
            .SetSmallIcon(Resource.Drawable.notification_action_background);

                    // Build the notification:
                    Notification notification = builder.Build();

                    // Get the notification manager:
                    NotificationManager notificationManager =
                        GetSystemService(Context.NotificationService) as NotificationManager;

                    // Publish the notification:
                    const int notificationId = 0;
                    notificationManager.Notify(notificationId, notification);
                    Thread.Sleep(2000);
                }
            });
            t.Start();

            





            return StartCommandResult.NotSticky;
        }
        
        void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");
        }
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = Resources.GetString(Resource.String.appbar_scrolling_view_behavior);
            var channelDescription = GetString(Resource.String.app_name);
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}