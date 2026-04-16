using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Class;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;




namespace reminder_schedule_backend.Services
{
    public class ClassService
    {
        private readonly AppDbContext _db;
        public ClassService(AppDbContext db) => _db = db;

        //---------------------------GET ALL CLASSES---------------------------
        public async Task<List<ClassResponseDto>> GetAllAync()
        {
            var result = await _db.Classses.ToListAsync();
            return result.Select(c => ToClassResponseDto(c)).ToList();

        }


        //---------------------------GET CLASS BY ID---------------------------
        public async Task<ClassResponseDto> GetByIdAsync(int id)
        {
            var result = await _db.Classses.FindAsync(id);
            if (result == null) throw new NotFoundException("Class not found");
            return ToClassResponseDto(result);
        }


        //---------------------------CREATE CLASS---------------------------
        public async Task<ClassResponseDto> CreateAsync(ClassCreateDto dto)
        {
            var c = new Class
            {
                Name = dto.Name,  
                Grade = dto.Grade
            };

            _db.Classses.Add(c);
            await _db.SaveChangesAsync();
            return ToClassResponseDto(c);
        }


        //---------------------------UPDATE CLASS---------------------------
        public async Task<ClassResponseDto> UpdateAsync(int id, ClassUpdateDto dto)
        {
            var result = await _db.Classses.FindAsync(id);
            if (result == null) throw new NotFoundException("Class not found");

            result.Name = dto.Name;
            result.Grade = dto.Grade;

            await _db.SaveChangesAsync();
            return ToClassResponseDto(result);
        }

        //---------------------------DELETE CLASS---------------------------
        public async Task DeleteAsync(int id)
        {
            var result = await _db.Classses.FindAsync(id);
            if (result == null) throw new NotFoundException("Class not found");

            _db.Classses.Remove(result);
            await _db.SaveChangesAsync();
        }



        //---------------------------CLASSES RESPONSE DTO----------------> -----------
        public static ClassResponseDto ToClassResponseDto(Class c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Grade = c.Grade
        };
    }
}
