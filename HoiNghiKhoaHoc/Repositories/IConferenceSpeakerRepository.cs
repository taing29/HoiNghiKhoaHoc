using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
	public interface IConferenceSpeakerRepository
	{
		Task<List<ConferenceSpeaker>> GetSpeakersByConferenceIdAsync(int conferenceId);
		
	}
}
