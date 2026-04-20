using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Task
{
    public class TaskCreateDto
    {
         [Required(ErrorMessage = "Nama/Deskripsi tugas harus diisi tugas harus diisi")]
         public string description { get; set; } = null!;
         [Required] public DateTime? reminderAt { get; set; }
         [Required(ErrorMessage = "Jadwal Id harus dikirim")]
         public int scheduleId { get; set; }
    }

    public class TaskUpdateDto
    {
        public string? description { get; set; } = null!; 
        public DateTime? reminderAt { get; set; }
    } 
    
    public class TaskResponseDto
    { 
        public int Id { get; set; }
        public string description { get; set; } = null!;
        public DateTime reminderAt { get; set; }
        public bool status { get; set; }
        public int scheduleId { get; set; }
        public string? teacherName { get; set; }
        public string? className { get; set; }
        public string? subjectName { get; set; }
        public TimeSpan? sessionStart{ get; set; }
        public DayOfWeek day { get; set; }

    }
}
