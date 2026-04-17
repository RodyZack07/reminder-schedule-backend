using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Session;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;

namespace reminder_schedule_backend.Services
{
    public class SessionService
    {
        public readonly AppDbContext _db;
        public SessionService(AppDbContext db)  => _db = db;


        //---------------------------GET ALL SESSIONS----------------------
        public async Task<List<SessionResponseDto>> GetAllSessions()
        {
            var sessions = await _db.Sessions.ToListAsync();
            return sessions.Select(ToSessionResponseDto).ToList();
        }

        //---------------------------GET SESSION BY ID----------------------
        public async Task<SessionResponseDto> GetSessionByIdAsync(int id)
        {
            var session = await _db.Sessions.FindAsync(id);
            if (session == null) throw new NotFoundException("Session not found");
            return ToSessionResponseDto(session);
        }


        //---------------------------CREATE SESSION----------------------
        public async Task<SessionResponseDto> CreateSessionAsync(SessionCreateDto dto)
        {
            if(dto.EndTime <= dto.StarTime) 
                throw new BadRequestException("End time must be greater than start time");

            if (await CheckConflict(dto.StarTime, dto.EndTime)) 
                throw new BadRequestException("Session time conflicts with an existing session");

            var session = new Session
            {
                Name = dto.Name,
                startTime = dto.StarTime,
                endTime = dto.EndTime
            };

            //save to db
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();

            return ToSessionResponseDto(session);

        }


        //-----------------------------UPDATE SESSION----------------------
        public async Task<SessionResponseDto> UpdateSessionAsync(int id, SessionUpdateDto dto)
        {
            var session = await _db.Sessions.FindAsync(id);

            if (session == null) throw new NotFoundException("Session not found");

            if(dto.EndTime <= dto.StarTime) 
                throw new BadRequestException("End time must be greater than start time"); 
            
            if (await CheckConflict(dto.StarTime, dto.EndTime))
                    throw new BadRequestException("Session time conflicts with an existing session");


            session.Name = dto.Name;
            session.startTime = dto.StarTime;
            session.endTime = dto.EndTime;

            //update to db
            await _db.SaveChangesAsync();

            return ToSessionResponseDto(session);
        }


        //---------------------------DELETE SESSION----------------------
        public async Task DeleteSession(int id)
        {
            var session = await _db.Sessions.FindAsync(id);
            if (session == null) throw new NotFoundException("Session not found");

            //delete from db
            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();
        }


        //---------------------------CHECK CONFLICT----------------------
        public async Task<bool> CheckConflict(TimeSpan startTime, TimeSpan endTime, int excludeId = 0)
        {
            return await _db.Sessions.AnyAsync(s =>
                s.Id != excludeId &&
                startTime < s.endTime &&
                endTime > s.startTime
            );
        }

        //---------------------------SESSION RESPONSE---------------------------
        public static SessionResponseDto ToSessionResponseDto(Session session) => new()
        {
            Id = session.Id,
            Name = session.Name,
            StarTime = session.startTime,
            EndTime = session.endTime
        };



    }
}
