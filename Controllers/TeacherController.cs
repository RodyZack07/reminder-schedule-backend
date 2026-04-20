using Microsoft.AspNetCore.Authorization;
using reminder_schedule_backend.Services;
using Microsoft.AspNetCore.Mvc;
using reminder_schedule_backend.DTOs.Teacher;


namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        public readonly TeacherService _service;
        public TeacherController(TeacherService service) => _service = service;

        //-------------------------------GET ALL TEACHERS-------------------------------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        { 
            var result = await _service.GetAllTeachersAsync();
            return Ok(new { success = true, message = $"{result.Count} guru ditemukan", data = result });
        }

        //-------------------------------GET TEACHER BY ID-------------------------------
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetTeacherByIdAsync(id);
            return Ok(new { success = true, message = $"Guru dengan id {id} ditemukan", data = result });
        }


        //-------------------------------CREATE TEACHER-------------------------------
        [HttpPost]
        [Authorize (Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] TeacherCreateDto dto)
        {
            var result = await _service.CreateTeacherAsync(dto);
            return Ok(new { success = true, message = $"Guru dengan NIK {dto.Nik} berhasil dibuat", data = result });
        }

        //-------------------------------UPDATE TEACHER-------------------------------
        [HttpPatch("{id:int}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TeacherUpdateDto dto)
        {
           var result = await _service.UpdateTeacherAsync(id, dto);
           return Ok(new { success = true, message = $"Guru dengan id {id} berhasil diperbarui", data = result });
        }

        //-------------------------------LOGIN TEACHER-------------------------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] TeacherLoginDto dto)
        {
            var result = await _service.LoginTeacherAsync(dto, Response);
            return Ok(new { success = true, message = "Login berhasil", data = result });
        }

        //-------------------------------REFRESH TOKEN-------------------------------
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _service.RefreshTokenAsync(Request, Response);
            return Ok(new { success = true, message = "Token berhasil diperbarui", data = result });
        }

        [HttpPatch("update-fcm-token")]
        [Authorize]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenDto dto)
        {
            // Ambil ID dari Token JWT yang sedang login
            var teacherId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            await _service.UpdateFcmTokenAsync(teacherId, dto.Token);

            return Ok(new { success = true, message = "FCM Token berhasil diperbarui" });
        }

        //-------------------------------LOGOUT TEACHER-------------------------------
        [HttpPost("logout")]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutTeacherAsync(Request, Response);
            return Ok(new { success = true, message = "Logout berhasil" });
        }

        //-------------------------------DELETE TEACHER-------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteTeacherAsync(id);
            return Ok(new { success = true, message = $"Guru dengan id {id} berhasil dihapus" });
        }
    }
}
