using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
    public class Conference
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; } 
        public string Description { get; set; }
        //public string? Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } 
        public string Organizer { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } 
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
