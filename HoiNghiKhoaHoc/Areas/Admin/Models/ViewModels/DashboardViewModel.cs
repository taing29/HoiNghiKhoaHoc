namespace HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalConferences { get; set; }
        public List<TopConferenceInfo> Top5Registered { get; set; }
        public List<TopConferenceInfo> Top5Favorited { get; set; }
    }
}
