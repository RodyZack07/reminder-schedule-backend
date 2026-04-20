using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace reminder_schedule_backend.Services
{
    public class FirebaseNotificationService
    {
        public FirebaseNotificationService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                GoogleCredential credential;

                // 1. Coba ambil dari Environment Variable (Untuk di Render)
                var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

                if (!string.IsNullOrEmpty(firebaseJson))
                {
                    // CARA BARU (Bebas Warning): Menggunakan CredentialFactory dari String JSON
                    var serviceCredential = CredentialFactory.FromJson<ServiceAccountCredential>(firebaseJson);
                    credential = serviceCredential.ToGoogleCredential();
                }
                else
                {
                    // CARA BARU (Bebas Warning): Menggunakan CredentialFactory langsung dari File
                    var serviceCredential = CredentialFactory.FromFile<ServiceAccountCredential>("service-account.json");
                    credential = serviceCredential.ToGoogleCredential();
                }

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential
                });
            }
        }
        public async Task SendNotificationAsync(string token, string title, string body, int? scheduleId = null)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>()
                {
                    { "scheduleId", scheduleId?.ToString() ?? "" },
                    { "click_action", "CREATE_TASK_REPLY" }
                },
                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification()
                    {
                        DefaultVibrateTimings = false,
                        VibrateTimingsMillis = new long[] { 200, 100, 200, 100, 200 },
                        Sound = "default",
                        ClickAction = "CREATE_TASK_REPLY"
                    }
                },
                Webpush = new WebpushConfig()
                {
                    Notification = new WebpushNotification()
                    {
                        Title = title,
                        Body = body,
                        Icon = "/icon-192x192.png",
                        Badge = "/icon-192x192.png"
                    },
                    Data = new Dictionary<string, string>()
                    {
                        { "scheduleId", scheduleId?.ToString() ?? "" }
                    }
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"[FCM] Notifikasi Sukses Terkirim! ID: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FCM Error] Gagal kirim notifikasi: {ex.Message}");
            }
        }
    }
}
