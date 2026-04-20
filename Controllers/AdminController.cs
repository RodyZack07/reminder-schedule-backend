using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using reminder_schedule_backend.DTOs.Admin;
using reminder_schedule_backend.Services;

namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _service;
        public AdminController(AdminService service) => _service = service;

        // ============================================================
        // POST /api/admins/register — buat akun admin
        // Untuk keamanan: di production, endpoint ini sebaiknya dinonaktifkan
        // setelah admin pertama dibuat, atau dilindungi dengan secret key
        // ============================================================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AdminRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Data tidak valid", errors = ModelState });

            var result = await _service.RegisterAsync(dto);
            return CreatedAtAction(nameof(Register),
                new { success = true, message = "Akun admin berhasil dibuat", data = result });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Data tidak valid", errors = ModelState });

            var result = await _service.LoginAsync(dto, Response);
            return Ok(new { success = true, message = "Login berhasil", data = result });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var result = await _service.RefreshTokenAsync(Request, Response);
            return Ok(new { success = true, message = "Token diperbarui", data = result });
        }

        [HttpPost("logout")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync(Request, Response);
            return Ok(new { success = true, message = "Logout berhasil" });
        }

        // ============================================================
        // GET /api/admins/me — profil admin dari token
        // ============================================================
        [HttpGet("me")]
        [Authorize(Roles = "admin")]
        public IActionResult GetMe()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                success = true,
                message = "Profil ditemukan",
                data = new { id, name, email }
            });
        }
    }
}
