using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using Microsoft.AspNetCore.Http;

	public class ConferenceEditViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Tiêu đề không được để trống.")]
		[StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự.")]
		[Display(Name = "Tiêu đề")]
		public string Title { get; set; }

		[StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự.")]
		[Display(Name = "Mô tả")]
		public string Description { get; set; }

		[Display(Name = "Nội dung chi tiết")]
		public string Content { get; set; }

		[Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
		[DataType(DataType.Date)]
		[Display(Name = "Ngày bắt đầu")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
		[DataType(DataType.Date)]
		[Display(Name = "Ngày kết thúc")]
		[DateGreaterThan("StartDate", ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu.")]
		public DateTime EndDate { get; set; }

		[Required(ErrorMessage = "Địa điểm không được để trống.")]
		[StringLength(300, ErrorMessage = "Địa điểm tối đa 300 ký tự.")]
		[Display(Name = "Địa điểm")]
		public string Location { get; set; }

		[Required(ErrorMessage = "Người tổ chức không được để trống.")]
		[StringLength(100, ErrorMessage = "Người tổ chức tối đa 100 ký tự.")]
		[Display(Name = "Người tổ chức")]
		public string Organizer { get; set; }

		[Display(Name = "Kích hoạt")]
		public bool IsActive { get; set; }

		[Required(ErrorMessage = "Vui lòng chọn danh mục.")]
		[Display(Name = "Danh mục")]
		public int CategoryId { get; set; }

		[Display(Name = "Ảnh banner")]
		public IFormFile? BannerImage { get; set; }

		public string? ExistingBannerImage { get; set; }
	}


}
