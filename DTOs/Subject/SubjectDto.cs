using System.ComponentModel.DataAnnotations;


namespace reminder_schedule_backend.DTOs.Subject
{
    public class SubjectCreateDto
    {
        [Required(ErrorMessage = "Subject Harus diisi")]
        [StringLength(100, MinimumLength = 1 , ErrorMessage = "Isi setidaknya 1 karakter")]
        public string Name { get; set; } = null!;
    }


    public class SubjectUpdateDto
    {
        [StringLength(100, MinimumLength = 1 , ErrorMessage = "Isi setidaknya 1 karakter")]
        public string Name { get; set; } = null!;
    }


    public class SubjectResponseDto 
    { 
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
