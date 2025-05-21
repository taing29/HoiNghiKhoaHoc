namespace HoiNghiKhoaHoc.Models.ViewModels
{
	public class ConferenceDetailViewModel
	{
		public Conference CurrentConference { get; set; }
		public IEnumerable<Conference> RelatedConferences { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsRegistered { get; internal set; }
    }
}
