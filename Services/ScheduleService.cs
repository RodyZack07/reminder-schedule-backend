using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NpgsqlTypes;
using reminder_schedule_backend.Helpers;
using reminder_schedule_backend.Data;   
using reminder_schedule_backend.DTOs.Schedule;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;


namespace reminder_schedule_backend.Services
{
    public class ScheduleService
    {
        private readonly AppDbContext _db;

        public ScheduleService(AppDbContext db) => _db = db;

        //---------------------------GET ALL SCHEDULES---------------------------
        public async Task<List<ScheduleResponseDto>> GetAllAsync()
        {
                var schedules = await _db.Schedules
                    .Include(t => t.teacher)
                    .Include(s => s.Class)
                    .Include(s => s.subject)
                    .Include(s => s.sessionStart)
                    .Include(s => s.sessionEnd)
                    .OrderBy(s => s.day)
                    .ThenBy(s => s.sessionStart.startTime)
                    .ToListAsync();

            return schedules.Select(ToScheduleResponseDto).ToList();
        }

        //---------------------------GET SCHEDULE BY ID---------------------------
        public async Task<ScheduleResponseDto> GetByIdAsync(int id)
        {
            var schedule = await _db.Schedules
                .Where(s => s.id == id)
                .Include(s => s.teacher)
                .Include(s => s.Class)
                .Include(s => s.subject)
                .Include(s => s.sessionStart)
                .Include(s => s.sessionEnd)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Jadwal tidak ditemukan");
            return ToScheduleResponseDto(schedule);
        }

        //---------------------------GET SCHEDULE BY TEACHER---------------------------
        public async Task<List<ScheduleResponseDto>> GetScheduleByTeacherAsync(int teacherId)
        {
            if (!await _db.Teachers.AnyAsync(t => t.Id == teacherId))
                throw new NotFoundException("Guru tidak ditemukan");

            var schedules = await _db.Schedules
                .Where(s => s.teacherId == teacherId)
                .Include(s => s.teacher)
                .Include(s => s.Class)
                .Include(s => s.subject)
                .Include(s => s.sessionStart)
                .Include(s => s.sessionEnd)
                .OrderBy(s => s.day)
                .ThenBy(s => s.sessionStart.startTime)
                .ToListAsync();

            return schedules.Select(ToScheduleResponseDto).ToList();
        }

        //---------------------------GET SCHEDULE BY TODAY---------------------
        public async Task<List<ScheduleResponseDto>> GetScheduleTodayAsync(int teacherId)
        {
            var today = DateTime.Today.DayOfWeek;
            var schedules = await _db.Schedules
                .Where(s => s.teacherId == teacherId && s.day == today)
                .Include(s => s.teacher)
                .Include(s => s.Class)
                .Include(s => s.subject)
                .Include(s => s.sessionStart)
                .Include(s => s.sessionEnd)
                .OrderBy(s => s.sessionStart.startTime)
                .ToListAsync();
            return schedules.Select(ToScheduleResponseDto).ToList();
        }

        //---------------------------CREATE SCHEDULE---------------------------
        public async Task<ScheduleResponseDto> CreateScheduleAsync (ScheduleCreateDto dto)
        {
            await ValidateForeignKey(dto.teacherId, dto.classId, dto.subjectId, dto.sessionStartId, dto.sessionEndId);
            await ValidateSessionOrderAsync(dto.sessionStartId, dto.sessionEndId);
            await CheckConflictAsync((DayOfWeek)dto.day, dto.sessionStartId, dto.sessionEndId, dto.classId, dto.teacherId);


            var schedule = new Schedule
            {
                day = (DayOfWeek)dto.day,
                teacherId = dto.teacherId,
                classId = dto.classId,
                subjectId = dto.subjectId,
                sessionStartId = dto.sessionStartId,
                sessionEndId = dto.sessionEndId
            };
            
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
            await LoadRelationAsync(schedule);

            return ToScheduleResponseDto(schedule);
        }

        //---------------------------UPDATE SCHEDULE---------------------------
        public async Task<ScheduleResponseDto> UpdateScheduleAsync(int id, ScheduleUpdateDto dto)
        {
            var schedule = await _db.Schedules
               .Include(s => s.teacher)
               .Include(s => s.Class)
               .Include(s => s.subject)
               .Include(s => s.sessionStart)
               .Include(s => s.sessionEnd)
               .FirstOrDefaultAsync(s => s.id == id)
               ?? throw new NotFoundException("Jadwal tidak ditemukan");

            // Validasi FK hanya untuk field yang dikirim
            if (dto.teacherId.HasValue && !await _db.Teachers.AnyAsync(t => t.Id == dto.teacherId.Value))
                throw new NotFoundException("Guru tidak ditemukan");
            if (dto.classId.HasValue && !await _db.Classes.AnyAsync(c => c.Id == dto.classId.Value))
                throw new NotFoundException("Kelas tidak ditemukan");
            if (dto.subjectId.HasValue && !await _db.Subjects.AnyAsync(s => s.Id == dto.subjectId.Value))
                throw new NotFoundException("Mata pelajaran tidak ditemukan");
            if (dto.sessionStartId.HasValue && !await _db.Sessions.AnyAsync(s => s.Id == dto.sessionStartId.Value))
                throw new NotFoundException("Sesi mulai tidak ditemukan");
            if (dto.sessionEndId.HasValue && !await _db.Sessions.AnyAsync(s => s.Id == dto.sessionEndId.Value))
                throw new NotFoundException("Sesi selesai tidak ditemukan");

            // Nilai final
            var newTeacherId = dto.teacherId ?? schedule.teacherId;
            var newClassId = dto.classId ?? schedule.classId;
            var newSubjectId = dto.subjectId ?? schedule.subjectId;
            var newSessionStartId = dto.sessionStartId ?? schedule.sessionStartId;
            var newSessionEndId = dto.sessionEndId ?? schedule.sessionEndId;
            var newDay = dto.day.HasValue ? (DayOfWeek)dto.day.Value : schedule.day;

            await ValidateSessionOrderAsync(newSessionStartId, newSessionEndId);
            await CheckConflictAsync(newDay, newSessionStartId, newSessionEndId,
                newClassId, newTeacherId, id);

            schedule.teacherId = newTeacherId;
            schedule.classId = newClassId;
            schedule.subjectId = newSubjectId;
            schedule.sessionStartId = newSessionStartId;
            schedule.sessionEndId = newSessionEndId;
            schedule.day = newDay;

            await _db.SaveChangesAsync();
            await LoadRelationAsync(schedule);

            return ToScheduleResponseDto(schedule);
        }

        //---------------------------DELETE SCHEDULE---------------------------
        public async Task DeleteScheduleAsync(int id)
        {
            var schedule = await _db.Schedules.FindAsync(id);
            if (schedule == null)
                throw new NotFoundException(" Jadwal tidak ditemukan");


            _db.Schedules.Remove(schedule);
            await _db.SaveChangesAsync();
        }

        //---------------------------CHECK FOREIGN KEY VALIDATION---------------------------
        private async Task ValidateForeignKey (int teacherId, int classId, int subjectId, int sessionStartId, int sessionEndId)
        {
            if (!await _db.Teachers.AnyAsync(t => t.Id == teacherId))
                throw new NotFoundException("Guru tidak ditemukan");
            if (!await _db.Classes.AnyAsync(c => c.Id == classId))
                throw new NotFoundException("Kelas tidak ditemukan");
            if (!await _db.Subjects.AnyAsync(s => s.Id == subjectId))   
                throw new NotFoundException("Mata pelajaran tidak ditemukan");
            if (!await _db.Sessions.AnyAsync(s => s.Id == sessionStartId))
                throw new NotFoundException("Sesi mulai tidak ditemukan");
            if (!await _db.Sessions.AnyAsync(s => s.Id == sessionEndId))
                throw new NotFoundException("Sesi akhir tidak ditemukan");
        }

        private async Task ValidateSessionOrderAsync(int sessionStartId, int sessionEndId)
        {
            var start = await _db.Sessions.FindAsync(sessionStartId);
            var end = await _db.Sessions.FindAsync(sessionEndId);

            if (start != null && end != null && start.startTime > end.startTime)
                throw new BadRequestException("Sesi mulai tidak boleh lebih akhir dari sesi selesai");
        }


        //---------------------------CHECK SCHEDULE CONFLICT---------------------------
        private async Task CheckConflictAsync(
            DayOfWeek day, int sessionStartId, int sessionEndId,
            int classId, int teacherId, int excludeId = 0)
        {
            
            if (await _db.Schedules.AnyAsync(s =>
                s.id != excludeId &&
                s.day == day &&
                s.classId == classId &&
                s.sessionStartId <= sessionEndId &&
                s.sessionEndId >= sessionStartId))
                throw new ConflictException("Kelas ini sudah memiliki jadwal di hari dan sesi yang sama");

            if (await _db.Schedules.AnyAsync(s =>
                s.id != excludeId &&
                s.day == day &&
                s.teacherId == teacherId &&
                s.sessionStartId <= sessionEndId &&
                s.sessionEndId >= sessionStartId))
                throw new ConflictException("Guru ini sudah mengajar di hari dan sesi yang sama");
        }

        //--------------------LOAD RELATION CLASS---------------------//
        private async Task LoadRelationAsync(Schedule s)
        {
            await _db.Entry(s).Reference(sc => sc.teacher).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.Class).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.subject).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionStart).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionEnd).LoadAsync();
        }


        //--------------------RESPONSE DARI SCHEDLE KE SCHEDULE RESPONSE DTO---------------------//
        private static ScheduleResponseDto ToScheduleResponseDto(Schedule s) => new()
        {   
            id = s.id,
            day = s.day,
            teacherId = s.teacherId,
            teacherName = s.teacher.Name,
            classId = s.classId,
            className = s.Class?.Name,
            subjectId = s.subjectId,
            subjectName = s.subject?.Name,

            sessionStartId = s.sessionStartId,
            sessionStartName = s.sessionStart?.Name,
            sessionEndId = s.sessionEndId,
            sessionEndName = s.sessionEnd?.Name,

            startTime = s.sessionStart?.startTime,
            endTime = s.sessionEnd?.endTime

        };

    }
}
