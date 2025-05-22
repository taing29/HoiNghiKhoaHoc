using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFRegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public EFRegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ConferenceRegistration> GetRegistrationAsync(string userId, int conferenceId)
        {
            return await _context.ConferenceRegistrations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ConferenceId == conferenceId);
        }

        public async Task RegisterAsync(ConferenceRegistration registration)
        {
            _context.ConferenceRegistrations.Add(registration);
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int registrationId)
        {
            var registration = await _context.ConferenceRegistrations.FindAsync(registrationId);
            if (registration != null)
            {
                _context.ConferenceRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ConferenceRegistration>> GetRegistrationsByUserIdAsync(string userId)
        {
            return await _context.ConferenceRegistrations
                .Include(r => r.Conference)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}
