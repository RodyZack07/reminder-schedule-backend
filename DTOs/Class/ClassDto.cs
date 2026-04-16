using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Class
{
    public class ClassCreateDto
    {
        [Required(ErrorMessage = "Nama kelas harus diisi")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Buat setidaknya 1 sesi")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Grade kelas harus diisi")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Buat setidaknya 1 sesi")]
        public string Grade { get; set; } = null!;
    }


    public class ClassUpdateDto
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Buat setidaknya 1 sesi")]
        public string Name { get; set; } = null!;

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Buat setidaknya 1 sesi")]
        public string Grade{ get; set; } = null!;
    }

    public class ClassResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Grade { get; set; } = null!;
    }
}

