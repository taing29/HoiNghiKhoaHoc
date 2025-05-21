namespace HoiNghiKhoaHoc.Models.ViewModels
{
	public class ConferenceDetailViewModel
	{
		public Conference CurrentConference { get; set; }
		public IEnumerable<Conference> RelatedConferences { get; set; }
	}
}
