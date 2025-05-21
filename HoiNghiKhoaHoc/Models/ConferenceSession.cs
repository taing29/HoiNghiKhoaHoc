namespace HoiNghiKhoaHoc.Models
{
	public class ConferenceSession
	{
		public int Id { get; set; }
		public int ConferenceId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Location { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public Conference Conference { get; set; }
	}
}
