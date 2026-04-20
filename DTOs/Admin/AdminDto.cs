using System.ComponentModel.DataAnnotations;

namespace reminder_schedule_backend.DTOs.Admin
{
    public class AdminRegisterDto
    {
        [Required(ErrorMessage = "Nama harus diisi")]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Minimal 6 karakter")]
        public string Password { get; set; } = null!;
    }

    public class AdminLoginDto
    {
        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password harus diisi")]
        public string Password { get; set; } = null!;
    }

    public class AdminLoginResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class AdminResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
