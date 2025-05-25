using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
	public class ConferenceReview
	{
		public int Id { get; set; }

		[Required]
		public int ConferenceId { get; set; }
		public Conference Conference { get; set; }

		[Required]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }

		[Required]
		[StringLength(1000)]
		public string Content { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
