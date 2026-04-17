using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using reminder_schedule_backend.Services;
using reminder_schedule_backend.DTOs.Class;



namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/classes")]
    [Authorize]
    public class ClassController : ControllerBase
    {

        public readonly ClassService _service;
        public ClassController(ClassService service) => _service = service;

        //---------------------------GET ALL CLASSES---------------------------
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ClassResponseDto>>> GetAllAsync()
        {
            var result = await _service.GetAllAync();
            return Ok(new { success = true, message = "Mata pelajaran berhasil diambil", data = result });
        }

        //---------------------------GET CLASS BY ID---------------------------
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetByID(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new { success = true, message = $"Kelas dengan id {id} ditemukan", data = result });
        }

        //---------------------------CREATE CLASS---------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ClassCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(new { success = true, message = $"Kelas dengan nama {dto.Name} berhasil dibuat", data = result });
        }

        //---------------------------UPDATE CLASS---------------------------
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(new { success = true, message = $"Kelas dengan id {id} berhasil diperbarui", data = result });
        }

        //---------------------------DELETE CLASS---------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        { 
            await _service.DeleteAsync(id);
            return Ok(new { success = true, message = $"Kelas dengan id {id} berhasil dihapus" });
        }
    }
}
