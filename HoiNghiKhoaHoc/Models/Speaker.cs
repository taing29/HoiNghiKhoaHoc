using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
	public class Speaker
	{
		public int Id { get; set; }

		[Required, MaxLength(200)]
		public string FullName { get; set; }

		[MaxLength(100)]
		public string? Title { get; set; }

		[MaxLength(200)]
		public string? Affiliation { get; set; }

		public string? Bio { get; set; }

		[MaxLength(300)]
		public string? PhotoUrl { get; set; }
        [Required]
        public string Email { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Navigation
		[BindNever]
		public ICollection<ConferenceSpeaker>? ConferenceSpeakers { get; set; }
	}
}
