using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
	public interface IConferenceReviewRepository
	{
		Task<IEnumerable<ConferenceReview>> GetReviewsByConferenceIdAsync(int conferenceId);
		Task AddReviewAsync(ConferenceReview review);
	}
}
