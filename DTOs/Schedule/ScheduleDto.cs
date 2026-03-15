using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace reminder_schedule_backend.DTOs.Schedule
{
    public class ScheduleCreateDto
    {
        [Required(ErrorMessage = "Subject harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = " SubjectID tidak valid")]
        public int subjectId { get; set; }

        [Required(ErrorMessage = "Sesi harus dipilih")]
        [Range(1, int.MaxValue, ErrorMessage = "SessionID tidak valid")]
        public int sessionId { get; set;}

        [Required(ErrorMessage =" Hari harus dipilih")]
        [Range(1, 7, ErrorMessage =" Pilih hari 1 - 7")]
        public int day { get; set; }
  
    }

    public class ScheduleUpdateDto 
    {
        
        [Range(1, int.MaxValue, ErrorMessage = " SubjectID tidak valid")]
        public int? subjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SessionID tidak valid")]
        public int? sessionId { get; set; }

     }

    public class ScheduleResponseDto { 

        public int subjectId { get; set; }
        public string? subjectName { get; set; }

        public int sessioId { get; set; }
        public string? sessionName { get; set; }

        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }



    }
}
