using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Task;
using reminder_schedule_backend.Exceptions;
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
                .Include(t => t.Schedule).ThenInclude(s => s.sessionStart)
                .Where(t => t.Schedule.teacherId == teacherId)
                .ToListAsync();
            return tasks.Select(ToTaskResponseDto).ToList();
        }

        //---------------------------CREATE TASK----------------------
        public async Task<TaskResponseDto> CreateTask(TaskCreateDto dto )
        {
           
            var currentSchedule = await _db.Schedules.FirstOrDefaultAsync(s => s.id == dto.scheduleId);

            if (currentSchedule == null)
                throw new NotFoundException("Jadwal tidak ditemukan"); 

            DateTime finalRemindAt;

            if (dto.reminderAt.HasValue)
            {
                finalRemindAt = dto.reminderAt.Value;
            }
            else
            {
                // 2. Cari semua hari di mana mapel ini diajarkan di KELAS yang sama
                var subjectDays = await _db.Schedules
                    .Where(s => s.subjectId == currentSchedule.subjectId && s.classId == currentSchedule.classId)
                    .Select(s => s.day)
                    .ToListAsync();

                DateTime today = DateTime.Today;
                int currentDayVal = (int)today.DayOfWeek;
                
                int minDaysUntilNext = 7;

                // 3. Looping semua hari mapel tersebut untuk mencari yang paling dekat
                foreach (var day in subjectDays)
                {
                    int targetDayVal = (int)day;
                    int daysUntil = (targetDayVal - currentDayVal + 7) % 7;

                    // Jika jadwalnya jatuh di hari yang sama dengan hari ini, anggap untuk minggu depan (+7)
                    if (daysUntil == 0)
                    {
                        daysUntil = 7;
                    }

                    // Simpan jarak hari yang paling kecil (paling dekat)
                    if (daysUntil < minDaysUntilNext)
                    {
                        minDaysUntilNext = daysUntil;
                    }
                }
                finalRemindAt = today.AddDays(minDaysUntilNext);
            }

            var task = new TaskReminder
            {
                description = dto.description,
                remindAt = finalRemindAt, 
                status = false,
                scheduleId = dto.scheduleId
            };

            _db.TaskReminders.Add(task);
            await _db.SaveChangesAsync();


            await _db.Entry(task).Reference(t => t.Schedule).Query()
                     .Include(s => s.Class)
                     .Include(s => s.teacher)
                     .Include(s => s.subject)
                     .Include(s => s.sessionStart) 
                     .LoadAsync();

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


        //---------------------------RESPONSE DTO----------------------
        public static TaskResponseDto ToTaskResponseDto(TaskReminder task) => new()
        {

            Id = task.Id,
            description = task.description,
            reminderAt = task.remindAt,
            status = task.status,
            scheduleId = task.scheduleId,

            subjectName = task.Schedule?.subject?.Name,
            teacherName = task.Schedule?.teacher?.Name,
            className = task.Schedule?.Class?.Name,

            sessionStart = task.Schedule?.sessionStart?.startTime,

            day = task.Schedule?.day ?? default,

        };
    }
}
