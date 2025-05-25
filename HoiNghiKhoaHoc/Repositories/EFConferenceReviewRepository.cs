using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFConferenceReviewRepository : IConferenceReviewRepository
	{
		private readonly ApplicationDbContext _context;

		public EFConferenceReviewRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ConferenceReview>> GetReviewsByConferenceIdAsync(int conferenceId)
		{
			return await _context.ConferenceReviews
				.Where(r => r.ConferenceId == conferenceId)
				.Include(r => r.User)
				.OrderByDescending(r => r.CreatedAt)
				.ToListAsync();
		}

		public async Task AddReviewAsync(ConferenceReview review)
		{
			_context.ConferenceReviews.Add(review);
			await _context.SaveChangesAsync();
		}
	}
}
