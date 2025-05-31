using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models.ViewModels
{
	public class ConferenceRegistrationView
	{
		public int ConferenceId { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
		public string PhoneNumber { get; set; }
		public Conference? Conference { get; set; }

	}
}
