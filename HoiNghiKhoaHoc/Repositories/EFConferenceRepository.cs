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

		public async Task<IEnumerable<Conference>> GetAllConferencesGlobalAsync()
		{
			return await _context.Conferences
				.Include(c => c.Country)
				.Where(c => c.Country != null && !c.Country.IsVietnam)
				.OrderByDescending(c => c.StartDate)
				.ToListAsync();
		}

		public async Task<IEnumerable<Conference>> GetAllConferencesPastAsync()
		{
			return await _context.Conferences
				.Where(c => c.EndDate < DateTime.Now)
				.OrderByDescending(c => c.EndDate)
				.Include(c => c.Category)
				.ToListAsync();
		}

		public async Task<IEnumerable<Conference>> GetAllConferencesUpcomingAsync()
		{
			return await _context.Conferences
				.Where(c => c.StartDate > DateTime.Now)
				.OrderBy(c => c.StartDate)
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

		public async Task<IEnumerable<Conference>> GetConferenceByIdCategoryAsync(Conference conference)
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
    }
}