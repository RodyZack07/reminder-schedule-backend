using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using reminder_schedule_backend.DTOs.Session;
using reminder_schedule_backend.Services;


namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        public readonly SessionService _service;
        public SessionController(SessionService service ) => _service = service;
        //-------------------------------GET ALL SESSIONS-------------------------------
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll ()
        {
            var result = await _service.GetAllSessions();
            return Ok(new { success = true, message = $"{result.Count} jadwal ditemukan", data = result });
        }

        //-------------------------------GET SESSION BY ID-------------------------------
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
      

        //-------------------------------CREATE SESSION-------------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] SessionCreateDto dto)
        {
            var result = await _service.CreateSessionAsync(dto);
            return CreatedAtAction(nameof(GetAll),
                new { id = result.Id },
                new { success = true, message = "Sesi berhasil dibuat", data = result });
        }

        //-------------------------------UPDATE SESSION-------------------------------
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] SessionUpdateDto dto)
        {
            var result = await _service.UpdateSessionAsync(id, dto);
            return Ok(new { success = true, message = "Sesi berhasil diperbarui", data = result });
        }

        //-------------------------------DELETE SESSION-------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSession(id);
            return Ok(new { success = true, message = "Sesi berhasil dihapus" });
        }
    }
}
