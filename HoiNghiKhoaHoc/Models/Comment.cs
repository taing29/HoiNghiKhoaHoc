using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
	public class Comment
	{
		public int Id { get; set; }

		public string Content { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[Required]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		[Required]
		public int ConferenceId { get; set; }
		public Conference Conference { get; set; }
	}
}
