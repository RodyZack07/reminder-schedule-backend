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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { success = true, message = $"{result.Count} jadwal ditemukan", data = result });
        }


        //-------------------------------GET SCHEDULE BY ID-------------------------------
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetById(int id)
        {

        }

        //-------------------------------CREATE SCHEDULE-------------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateDto dto)
        {
            var result = await _service.CreateScheduleAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = result.id },
                new { success = true, message = "Jadwal berhasil dibuat", data = result });)
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
