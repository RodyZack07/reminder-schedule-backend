using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Task;
using reminder_schedule_backend.Models;




namespace reminder_schedule_backend.Services
{
    public class TaskService
    {

        public readonly AppDbContext _db;
        public TaskService(AppDbContext db) => _db = db;

        //---------------------------GET ALL TASKS----------------------
        public async Task<List<TaskResponseDto>> GetAllTasks()
        {
            var tasks = await _db.TaskReminders.Include(t => t.Schedule).ThenInclude(s => s.Class)
                .Include(t => t.Schedule).ThenInclude(s => s.teacher)
                .Include(t => t.Schedule).ThenInclude(s => s.subject)
                .ToListAsync();
            return tasks.Select(ToTaskResponseDto).ToList();
        }

        //---------------------------GET TASK BY ID----------------------
        public async Task<TaskResponseDto?> GetTaskById(int id)
        {
            var task = await _db.TaskReminders.Include(t => t.Schedule).ThenInclude(s => s.Class)
                .Include(t => t.Schedule).ThenInclude(s => s.teacher)
                .Include(t => t.Schedule).ThenInclude(s => s.subject)
                .FirstOrDefaultAsync(t => t.Id == id);
            return task is null ? null : ToTaskResponseDto(task);
        }

        //---------------------------GET TASK BY TEACHER ID----------------------
        public async Task<List<TaskResponseDto>> GetTasksByTeacherId(int teacherId)
        {
            var tasks = await _db.TaskReminders.Include(t => t.Schedule).ThenInclude(s => s.Class)
                .Include(t => t.Schedule).ThenInclude(s => s.teacher)
                .Include(t => t.Schedule).ThenInclude(s => s.subject)
                .Where(t => t.Schedule.teacherId == teacherId)
                .ToListAsync();
            return tasks.Select(ToTaskResponseDto).ToList();
        }

        //---------------------------CREATE TASK----------------------
        public async Task<TaskResponseDto> CreateTask(TaskCreateDto dto )
        {
            var task = new TaskReminder
            {
                description = dto.description,
                remindAt = dto.reminderAt,
                status = false,
                
            };

            _db.TaskReminders.Add(task);
            await _db.SaveChangesAsync();
            return ToTaskResponseDto(task);
        }

        //---------------------------DELETE TASK----------------------
        public async Task DeleteTask (int id)
        {
            var task = await _db.TaskReminders.FindAsync(id);
            if (task == null) throw new Exception("Task not found");

            _db.TaskReminders.Remove(task);
            await _db.SaveChangesAsync();
        }

        public static TaskResponseDto ToTaskResponseDto(TaskReminder task) => new()
        {

            Id = task.Id,
            description = task.description,
            reminderAt = task.remindAt,
            status = task.status,
            scheduleId = task.scheduleId,
            subjectName = task.Schedule.subject.Name,
            teacherName = task.Schedule.teacher.Name,
            className = task.Schedule.Class.Name,
            sessionStart = task.Schedule.sessionStart,
            day = task.Schedule.day,

        };
    }
}
