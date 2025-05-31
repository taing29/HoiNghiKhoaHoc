namespace HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalConferences { get; set; }
        public int UpcomingConferences { get; set; }
        public int PastConferences { get; set; }
        public int InternationalConferences { get; set; }
        public List<TopConferenceInfo> Top5Registered { get; set; }
        public List<TopConferenceInfo> Top5Favorited { get; set; }
    }
}
