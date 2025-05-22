
namespace HoiNghiKhoaHoc.Models.ViewModels
{
	public class ConferenceDetailViewModel
	{
		public Conference CurrentConference { get; set; }
		public IEnumerable<Conference>? RelatedConferences { get; set; }
		public List<ConferenceSpeaker>? Speakers { get; set; }
		public IEnumerable<ConferenceSession> Sessions { get; set; }
		public bool IsPastConference => CurrentConference.EndDate < DateTime.Now;

	}
}
