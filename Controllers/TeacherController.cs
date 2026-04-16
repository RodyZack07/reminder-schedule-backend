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
