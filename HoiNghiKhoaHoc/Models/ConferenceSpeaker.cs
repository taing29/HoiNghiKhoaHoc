namespace HoiNghiKhoaHoc.Models
{
	public class ConferenceSpeaker
	{
		public int Id { get; set; }

		public int ConferenceId { get; set; }
		public Conference Conference { get; set; }

		public int SpeakerId { get; set; }
		public Speaker Speaker { get; set; }

		public bool IsKeynote { get; set; } = false;
		public bool IsPanelist { get; set; } = false;

		public int Order { get; set; } = 0;
	}
}
