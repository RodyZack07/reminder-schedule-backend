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
        }
    }
}
