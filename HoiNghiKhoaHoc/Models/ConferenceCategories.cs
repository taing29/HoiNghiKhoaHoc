namespace HoiNghiKhoaHoc.Models
{
    public class ConferenceCategories
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Conferences>? Conferences { get; set; }
    }
}
