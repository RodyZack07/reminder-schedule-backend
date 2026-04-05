using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NpgsqlTypes;
using reminder_schedule_backend.Helpers;
using reminder_schedule_backend.Data;   
using reminder_schedule_backend.DTOs.Schedule;
using reminder_schedule_backend.Models;

namespace reminder_schedule_backend.Services
{
    public class ScheduleService
    {
        private readonly AppDbContext _db;

        public ScheduleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<Schedule>>> GetAllSchedules()
        {
          
               var schedules = await _db.Schedules
                    .Include(s => s.Class)
                    .Include(s => s.subject)
                    .Include(s => s.session)
                    .ToListAsync();

            var result = schedules.Select(ScheduleResponseDto).ToList();

        }
        //---------------------------GET ALL SCHEDULES---------------------------
        
    }
}
