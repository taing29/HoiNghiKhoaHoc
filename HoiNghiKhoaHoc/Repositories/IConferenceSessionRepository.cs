using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
	public interface IConferenceSessionRepository
	{
		Task<IEnumerable<ConferenceSession>> GetByConferenceId(int Id);
	}
}
