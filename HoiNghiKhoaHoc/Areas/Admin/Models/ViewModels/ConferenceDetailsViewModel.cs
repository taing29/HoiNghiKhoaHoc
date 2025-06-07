using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels
{
    public class ConferenceDetailsViewModel
    {
        public Conference Conference { get; set; }
        public List<ConferenceRegistration> Registrations { get; set; }
    }
}
