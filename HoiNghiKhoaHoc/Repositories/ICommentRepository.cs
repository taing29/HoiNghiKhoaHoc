using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
	public interface ICommentRepository
	{
		Task<IEnumerable<Comment>> GetCommentsByConferenceIdAsync(int id);
		Task AddCommentAsync(Comment comment);
	}
}
