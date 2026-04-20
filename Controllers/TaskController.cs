using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using reminder_schedule_backend.Services;
using reminder_schedule_backend.DTOs.Task; 


namespace reminder_schedule_backend.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        public readonly TaskService _taskService;
        public TaskController(TaskService taskService) => _taskService = taskService;

        [HttpGet]
        public async Task<ActionResult<List<TaskResponseDto>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasks();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null) return NotFound(new { success = false, message = "Task not found" });
            return Ok(task);
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<List<TaskResponseDto>>> GetTasksByTeacherId(int teacherId)
        {
            var tasks = await _taskService.GetTasksByTeacherId(teacherId);
            return Ok(tasks);
        }   

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskCreateDto dto)
        {
            var task = await _taskService.CreateTask(dto);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTask(id);
            return Ok(new { success = true, message = "Task deleted successfully" });
        }
    }
}
