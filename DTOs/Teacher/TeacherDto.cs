using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Teacher
{
    public class TeacherCreateDto
    {
        [Required(ErrorMessage = "Nama guru harus diisi")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Isi setidaknya 1 karakter")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Isi setidaknya 6 karakter")]
        public string Password { get; set; } = null!;

    }

    public class TeacherUpdateDto
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Isi setidaknya 1 karakter")]
        public string Name { get; set; } = null!;

        [StringLength(200, MinimumLength = 6, ErrorMessage = "Isi setidaknya 6 karakter")]
        public string Password { get; set; } = null!;
    }

    public class TeacherResponseDto {

        public int Id { get; set; }
        public string Name { get; set; } = null!;

    }
}
