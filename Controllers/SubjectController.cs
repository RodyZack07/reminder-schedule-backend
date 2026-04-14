
using reminder_schedule_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using reminder_schedule_backend.DTOs.Subject;

namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        public readonly SubjectService _service;
        public SubjectController(SubjectService service) => _service = service;

        //-------------------------------GET ALL SUBJECTS-------------------------------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllSubjectsAsync();
            return Ok(new { success = true, message = $"{result.Count} mata pelajaran ditemukan", data = result });
        }

        //-------------------------------GET SUBJECT BY ID-------------------------------
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetSubjectByIdAsync(id);
            return Ok(new { success = true, message = $"Mata pelajaran dengan id {id} ditemukan", data = result });
        }

        //-------------------------------CREATE SUBJECT-------------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] SubjectCreateDto dto)
        {
            var result = await _service.CreateSubjectAsync(dto);
            return Ok(new { success = true, message = "Mata pelajaran berhasil dibuat", data = result });
        }

        //-------------------------------UPDATE SUBJECT-------------------------------
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectUpdateDto dto)
        {
            var result = await _service.UpdateSubjectAsync(id, dto);
            return Ok(new { success = true, message = "Mata pelajaran berhasil diperbarui", data = result });

        }

        //-------------------------------DELETE SUBJECT-------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSubjectAsync(id);
            return Ok(new { success = true, message = "Mata pelajaran berhasil dihapus" });
        }
    }
    }
