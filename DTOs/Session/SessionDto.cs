using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Session
{
    public class SessionCreateDto
    {
        
        [Required(ErrorMessage = " Start time darus diisi")]
        public TimeSpan StarTime { get; set; }

        [Required(ErrorMessage = " End time time darus diisi")]
        public TimeSpan EndTime { get; set; }

    }

    public class SessionResponseDto
    {
        public int Id { get; set; }
        public TimeSpan StarTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

}
