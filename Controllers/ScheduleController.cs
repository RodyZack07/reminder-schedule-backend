using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using reminder_schedule_backend.DTOs.Schedule;
using reminder_schedule_backend.Services;


namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    [Authorize]

    public class ScheduleController : ControllerBase
    {
        public readonly ScheduleService _service;
        public ScheduleController(ScheduleService service) => _service = service;

        //-------------------------------GET ALL SCHEDULES-------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { success = true, message = $"{result.Count} jadwal ditemukan", data = result });
        }


        //-------------------------------GET SCHEDULE BY ID-------------------------------
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new { success = true, message = $"Jadwal dengan id {id} ditemukan", data = result });
        }

        //------------------------------GET SCHEDULES BY TEACHER ID-------------------------------
        [HttpGet("teacher/{teacherId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByTeacherId(int teacherId)
        {
            var result = await _service.GetScheduleByTeacherAsync(teacherId);
            return Ok(new { success = true, message = $"{result.Count} jadwal ditemukan untuk guru dengan ID {teacherId}", data = result });
        }

        //------------------------------GET SCHEDULES BY TODAY-----------------------------------
        [HttpGet("teacher/{teacherId:int}/today")]
        [Authorize]
        public async Task<IActionResult> GetToday(int teacherId)
        {
            var result = await _service.GetScheduleTodayAsync(teacherId);
            return Ok(new { success = true, message = $"{result.Count} jadwal ditemukan untuk hari ini", data = result });
        }

        //-------------------------------CREATE SCHEDULE-----------------------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Data tidak valid", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var result = await _service.CreateScheduleAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = result.id },
                new { success = true, message = "Jadwal berhasil dibuat", data = result });
        }

        //-------------------------------UPDATE SCHEDULE-------------------------------
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin")]    
        public async Task<IActionResult> Update(int id, [FromBody] ScheduleUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Data tidak valid", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var result = await _service.UpdateScheduleAsync(id, dto);
            return Ok(new { success = true, message = "Jadwal berhasil diperbarui", data = result });


        }

        //-------------------------------DELETE SCHEDULE-------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteScheduleAsync(id);
            return Ok(new { success = true, message = "Jadwal berhasil dihapus" });
        }
    }

}
