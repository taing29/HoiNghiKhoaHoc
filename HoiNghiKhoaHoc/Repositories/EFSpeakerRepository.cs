using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFSpeakerRepository : ISpeakerRepository
    {
        private readonly ApplicationDbContext _context;
        public EFSpeakerRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddSpeakerAsync(Speaker speaker)
        {
            await _context.Speakers.AddAsync(speaker);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteSpeakerAsync(int id)
        {
            var speaker = await _context.Speakers.FindAsync(id);
            if (speaker != null)
            {
                _context.Speakers.Remove(speaker);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Speaker>> GetAllSpeakersAsync()
        {
            return await _context.Speakers
                .Include(s => s.ConferenceSpeakers) 
                .ToListAsync();
        }
        public async Task<Speaker?> GetSpeakerByIdAsync(int id)
        {
            return await _context.Speakers
                .Include(s => s.ConferenceSpeakers)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<IEnumerable<Speaker>> GetSpeakersByConferenceIdAsync(int conferenceId)
        {
            return await _context.Speakers
                .Where(s => s.Id == conferenceId)
                .ToListAsync();
        }
        public async Task UpdateSpeakerAsync(Speaker speaker)
        {
            var existingSpeaker = await _context.Speakers.FindAsync(speaker.Id);
            if (existingSpeaker != null)
            {
                _context.Entry(existingSpeaker).CurrentValues.SetValues(speaker);
                await _context.SaveChangesAsync();
            }
        }
    }
}
