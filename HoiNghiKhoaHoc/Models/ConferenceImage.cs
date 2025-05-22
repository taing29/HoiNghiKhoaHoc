using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
	public class ConferenceImage
	{
		public int Id { get; set; }

		[Required]
		public string ImageUrl { get; set; }

		// Khóa ngoại
		public int ConferenceId { get; set; }
		public Conference? Conference { get; set; }
	}
}
