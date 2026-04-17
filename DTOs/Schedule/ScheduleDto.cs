using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Schedule
{
    // ============================================================
    // CREATE DTO
    // ============================================================
    public class ScheduleCreateDto
    {
        [Required(ErrorMessage = "Guru harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "Guru Id tidak valid")]
        public int teacherId { get; set; }

        [Required(ErrorMessage = "Kelas harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "Kelas Id tidak valid")]
        public int classId { get; set; }

        [Required(ErrorMessage = "Mata pelajaran harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId tidak valid")]
        public int subjectId { get; set; }

        [Required(ErrorMessage = "Sesi mulai harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "SessionStartId tidak valid")]
        public int sessionStartId { get; set; }

        [Required(ErrorMessage = "Sesi selesai harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "SessionEndId tidak valid")]
        public int sessionEndId { get; set; }

        // DayOfWeek: 0=Minggu, 1=Senin, ..., 6=Sabtu
        [Required(ErrorMessage = "Hari harus dipilih")]
        [Range(0, 6, ErrorMessage = "Pilih hari 0-6")]
        public int day { get; set; }
    }

    // ============================================================
    // UPDATE DTO — semua nullable
    // ============================================================
    public class ScheduleUpdateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Guru Id tidak valid")]
        public int? teacherId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Kelas Id tidak valid")]
        public int? classId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SubjectId tidak valid")]
        public int? subjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SessionStartId tidak valid")]
        public int? sessionStartId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SessionEndId tidak valid")]
        public int? sessionEndId { get; set; }

        [Range(0, 6, ErrorMessage = "Pilih hari 0-6")]
        public int? day { get; set; }
    }

    // ============================================================
    // RESPONSE DTO
    // ============================================================
    public class ScheduleResponseDto
    {
        public int id { get; set; }
        public DayOfWeek day { get; set; }

        public int teacherId { get; set; }
        public string? teacherName { get; set; }

        public int classId { get; set; }
        public string? className { get; set; }

        public int subjectId { get; set; }
        public string? subjectName { get; set; }

        public int sessionStartId { get; set; }
        public string? sessionStartName { get; set; }   
        public int sessionEndId { get; set; }
        public string? sessionEndName { get; set; }     

        // Ambil dari sessionStart dan sessionEnd — tetap berguna untuk
        // MAUI app agar langsung tahu rentang jam tanpa parse nested object
        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }
    }
}