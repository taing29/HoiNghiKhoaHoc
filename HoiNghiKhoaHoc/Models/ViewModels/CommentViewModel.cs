using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models.ViewModels
{
	public class AddCommentViewModel
	{
		public int ConferenceId { get; set; }
		[Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
		public string Content { get; set; }
	}
}
