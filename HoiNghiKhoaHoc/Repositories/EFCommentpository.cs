using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFCommentRepository : ICommentRepository
	{
		private readonly ApplicationDbContext _context;

		public EFCommentRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddCommentAsync(Comment comment)
		{
			if (comment == null)
			{
				throw new ArgumentNullException(nameof(comment), "Comment cannot be null");
			}
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Comment>> GetCommentsByConferenceIdAsync(int id)
		{
			return await _context.Comments
				.Where(c => c.ConferenceId == id)
				.Include(c => c.User)
				.ToListAsync();
		}
	}
}
