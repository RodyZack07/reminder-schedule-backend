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
        public async Task SendNotificationAsync(string token, string title, string body)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification()
                    {
                        // Ganti baris VibrateConfig menjadi dua baris di bawah ini:
                        DefaultVibrateTimings = false,
                        VibrateTimingsMillis = new long[] { 200, 100, 200, 100, 200 },
                        Sound = "default"
                    }
                }
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"[FCM] Notifikasi Sukses Terkirim!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FCM Error] Gagal kirim notifikasi: {ex.Message}");
            }
        }
    }
}
