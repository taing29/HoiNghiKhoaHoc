using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
	public class Country
	{
		public int Id { get; set; }

		[Required, StringLength(100)]
		public string Name { get; set; }

		// Optional: thêm cờ để phân biệt trong nước/quốc tế
		public bool IsVietnam { get; set; } = false;

		// Navigation property
		public ICollection<Conference>? Conferences { get; set; }
	}
}
