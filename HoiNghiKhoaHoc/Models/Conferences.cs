namespace HoiNghiKhoaHoc.Models
{
    public class Conferences
    {
        public int ConferenceID { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } 
        public string Organizer { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } 
        public int CategoryId { get; set; }
        public ConferenceCategories? ConferenceCategories { get; set; }
    }
}
