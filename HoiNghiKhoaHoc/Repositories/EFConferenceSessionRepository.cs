using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFConferenceSessionRepository : IConferenceSessionRepository
	{
		private readonly ApplicationDbContext _context;
		public EFConferenceSessionRepository(ApplicationDbContext context)
		{
				_context = context;
		}
		public async Task<IEnumerable<ConferenceSession>> GetByConferenceId(int conferenceId)
		{
			return await _context.ConferenceSessions
						   .Where(s => s.ConferenceId == conferenceId)
						   .OrderBy(s => s.StartTime)
						   .ToListAsync();
		}
	}
}
