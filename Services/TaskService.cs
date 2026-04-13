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
