using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Subject;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;

namespace reminder_schedule_backend.Services
{
    public class SubjectService
    {
        public readonly AppDbContext _db;
        public SubjectService(AppDbContext db) => _db = db;

        //---------------------------GET ALL SUBJECTS----------------------


        //---------------------------CREATE SUBJECT------------------------
        public async Task<SubjectResponseDto> CreateSubjectAsync (SubjectCreateDto dto)
        {

            var subject = new Subject
            {
                Name = dto.Name
            };

            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();

            return ToSubjectResponseDto(subject);
        }


        //---------------------------DELETE SUBJECT------------------------
        public async Task DeleteSubjectAsync (int id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                throw new NotFoundException($"Subject with id {id} not found.");

            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
        }


        //---------------------------SUBJECT RESPPONSE ----------------------
        public static SubjectResponseDto ToSubjectResponseDto(Subject subject) => new()
        { 
            Id = subject.Id,
            Name = subject.Name
        };


    }
}
