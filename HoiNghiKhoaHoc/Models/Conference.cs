using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace HoiNghiKhoaHoc.Models
{
    public class Conference
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; } 
        public string Description { get; set; }
		public string? Content { get; set; }
		public string? BannerImage { get; set; }
		public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Organizer { get; set; }
        public bool IsActive { get; set; } = true;  
        ////để phân biệt trong nước hay quốc tế
        public bool IsInternational { get; set; }   
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        //phân biệt hội nghi miễn phí và có trả phí
        public bool IsPaid { get; set; } = false;
        //Phí tham gia hội nghị
		public decimal? Price { get; set; }
		public int CategoryId { get; set; }
		public Category? Category { get; set; }
	}
}
