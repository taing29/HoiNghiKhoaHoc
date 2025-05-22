using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFConferenceRepository : IConferenceRepository
    {
        private readonly ApplicationDbContext _context;

        public EFConferenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddConferenceAsync(Conference conference)
        {
            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConferenceAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference != null)
            {
                _context.Conferences.Remove(conference);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Conference> DisplayConferenceAsync(int id)
        {
            return await _context.Conferences
                         .Include(c => c.Category) 
                         .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesAsync()
        {
            return await _context.Conferences
            .Include(c => c.Category) // load luôn thông tin Category
            .ToListAsync();
        }

        public async Task<Conference> GetConferenceByIdAsync(int id)
        {
            return await _context.Conferences
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateConferenceAsync(Conference conference)
        {
            _context.Conferences.Update(conference);
            await _context.SaveChangesAsync();
        }
    }
}
