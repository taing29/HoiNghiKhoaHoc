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

        public Task AddConferenceAsync(Conference conference)
        {
            throw new NotImplementedException();
        }

        public Task DeleteConferenceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesAsync()
        {
            return await _context.Conferences
            .Include(c => c.Category) // load luôn thông tin Category
            .ToListAsync();
        }

        public Task<Conference> GetConferenceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateConferenceAsync(Conference conference)
        {
            throw new NotImplementedException();
        }
    }
}
