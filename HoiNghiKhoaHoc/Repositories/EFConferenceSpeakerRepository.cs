using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFConferenceSpeakerRepository : IConferenceSpeakerRepository
	{
		private readonly ApplicationDbContext _context;

		public EFConferenceSpeakerRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ConferenceSpeaker>> GetSpeakersByConferenceIdAsync(int Id)
		{
			return await _context.ConferenceSpeakers
				.Include(cs => cs.Speaker)
				.Where(cs => cs.ConferenceId == Id)
				.ToListAsync();
		}

		
	}

}
