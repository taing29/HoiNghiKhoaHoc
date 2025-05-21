using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFConferenceRepository : IConferenceRepository
    {
        private readonly ApplicationDbContext _context;

        public EFConferenceRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddConferenceAsync(Conference conference)
        {
            await _context.Conferences.AddAsync(conference);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConferenceAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference == null)
            {
                throw new Exception("Conference not found.");
            }
            _context.Conferences.Remove(conference);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesAsync()
        {
            return await _context.Conferences
                .Include(c => c.Category) // Load thông tin Category
                .ToListAsync();
        }

        public async Task<Conference> GetConferenceByIdAsync(int id)
        {
            var conference = await _context.Conferences
                .Include(c => c.Category) // Load thông tin Category
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conference == null)
            {
                throw new Exception("Conference not found.");
            }
            return conference;
        }

        public async Task<IEnumerable<Conference>> GetConferenceByIdCategory(Conference conference)
        {
			return await _context.Conferences
		.Where(c => c.Id != conference.Id && c.CategoryId == conference.CategoryId)
		.OrderByDescending(c => c.StartDate)
		.Take(4)
		.ToListAsync();
		}

		public async Task UpdateConferenceAsync(Conference conference)
        {
            var existingConference = await _context.Conferences.FindAsync(conference.Id);
            if (existingConference == null)
            {
                throw new Exception("Conference not found.");
            }
            _context.Entry(existingConference).CurrentValues.SetValues(conference);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Conference>> GetUpcomingConferencesAsync()
        {
            var now = DateTime.Now;
            return await _context.Conferences
                .Where(c => c.StartDate > now)
                .Include(c => c.Category)
                .Include(c => c.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> GetPastConferencesAsync()
        {
            var now = DateTime.Now;
            return await _context.Conferences
                .Where(c => c.EndDate < now)
                .Include(c => c.Category)
                .Include(c => c.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> GetInternationalConferencesAsync()
        {
            return await _context.Conferences
                .Where(c => c.IsInternational)
                .Include(c => c.Category)
                .Include(c => c.Images)
                .ToListAsync();
        }
    }
}