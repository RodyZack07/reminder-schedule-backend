using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;

namespace reminder_schedule_backend.Services
{
    public class ScheduleWatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduleWatcher> _logger;

        public ScheduleWatcher(IServiceScopeFactory scopeFactory, ILogger<ScheduleWatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Watcher] Alarm Otomatis Dimulai...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndSendNotifications();
                // Tunggu 1 menit sebelum cek lagi
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAndSendNotifications()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var firebase = scope.ServiceProvider.GetRequiredService<FirebaseNotificationService>();

            // 1. SETTING ZONA WAKTU
            var sekarangWIB = DateTime.UtcNow.AddHours(7);

            // Ini tetap kita simpan untuk teks pesan notifikasinya nanti
            var jamSkgStr = sekarangWIB.ToString("HH:mm");
            var hariSkg = sekarangWIB.DayOfWeek;

            // 2. AMBIL DATA JADWAL HARI INI
            var schedules = await db.Schedules
                .Include(s => s.teacher)
                .Include(s => s.subject)
                .Include(s => s.Class)
                .Include(s => s.sessionStart)
                .Where(s => s.day == hariSkg)
                .ToListAsync();

            // 3. FILTER JADWAL (FIX ERROR TIMESPAN)
            // Karena startTime adalah TimeSpan, kita cocokkan Jam dan Menit-nya secara matematik
            var schedulesMulaiSekarang = schedules.Where(s =>
                s.sessionStart != null &&
                s.sessionStart.startTime.Hours == sekarangWIB.Hour &&
                s.sessionStart.startTime.Minutes == sekarangWIB.Minute
            ).ToList();

            // 4. KIRIM NOTIFIKASI
            foreach (var s in schedulesMulaiSekarang)
            {
                if (!string.IsNullOrEmpty(s.teacher?.fcmToken))
                {
                    try
                    {
                        await firebase.SendNotificationAsync(
                            s.teacher.fcmToken,
                            "🔔 Waktunya Mengajar!",
                            $"Pak/Bu {s.teacher.Name}, sesi {s.subject?.Name} di kelas {s.Class?.Name} dimulai sekarang ({jamSkgStr})."
                        );
                        _logger.LogInformation($"Notif terkirim ke {s.teacher.Name} untuk mapel {s.subject?.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Gagal kirim notif ke {s.teacher.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}