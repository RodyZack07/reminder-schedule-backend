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
                .Include(s => s.sessionEnd) // TAMBAHKAN INI
                .Where(s => s.day == hariSkg)
                .ToListAsync();

            // 3. LOGIKA NOTIFIKASI MULAI MENGAJAR
            var schedulesMulaiSekarang = schedules.Where(s =>
                s.sessionStart != null &&
                s.sessionStart.startTime.Hours == sekarangWIB.Hour &&
                s.sessionStart.startTime.Minutes == sekarangWIB.Minute
            ).ToList();

            foreach (var s in schedulesMulaiSekarang)
            {
                await SendFcmSafe(firebase, s.teacher?.fcmToken, "🔔 Waktunya Mengajar!", 
                    $"Pak/Bu {s.teacher?.Name}, sesi {s.subject?.Name} di kelas {s.Class?.Name} dimulai sekarang ({jamSkgStr}).", s.teacher?.Name, s.id);
            }

            // 4. LOGIKA NOTIFIKASI PENGINGAT TUGAS (5 MENIT SEBELUM SELESAI)
            var limaMenitLagi = sekarangWIB.AddMinutes(5);
            var schedulesSelesaiLimaMenitLagi = schedules.Where(s =>
                s.sessionEnd != null &&
                s.sessionEnd.endTime.Hours == limaMenitLagi.Hour &&
                s.sessionEnd.endTime.Minutes == limaMenitLagi.Minute
            ).ToList();

            foreach (var s in schedulesSelesaiLimaMenitLagi)
            {
                await SendFcmSafe(firebase, s.teacher?.fcmToken, "🔔 Sesi Hampir Berakhir!", 
                    $"Pak/Bu {s.teacher?.Name}, sesi {s.subject?.Name} akan berakhir dalam 5 menit. Jangan lupa catat tugas jika ada!", s.teacher?.Name, s.id);
            }
        }

        private async Task SendFcmSafe(FirebaseNotificationService firebase, string token, string title, string body, string teacherName, int? scheduleId = null)
        {
            if (string.IsNullOrEmpty(token)) return;
            try
            {
                await firebase.SendNotificationAsync(token, title, body, scheduleId);
                _logger.LogInformation($"[Watcher] Notif '{title}' terkirim ke {teacherName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Watcher] Gagal kirim notif ke {teacherName}: {ex.Message}");
            }
        }
    }
}