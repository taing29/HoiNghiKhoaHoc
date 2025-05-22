using HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels;
using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ConferencesController : Controller
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly ICategoryRepository _categoryRepository;
		private readonly IWebHostEnvironment _env;

		public ConferencesController(IConferenceRepository conferenceRepository, ICategoryRepository categoryRepository, IWebHostEnvironment env)
        {
            _conferenceRepository = conferenceRepository ?? throw new ArgumentNullException(nameof(conferenceRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _env = env;
        }

        // Hiển thị danh sách hội nghị
        public async Task<IActionResult> Index()
        {
            var conferences = await _conferenceRepository.GetAllConferencesAsync();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(conferences);
        }

        // Hiển thị form tạo mới
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
			ViewBag.Categories = new SelectList(categories, "Id", "Name");
			return View();
        }

        // Xử lý tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConferenceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? fileName = null;
                if (model.BannerImage != null)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerImage.FileName);
                    var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.BannerImage.CopyToAsync(stream);
                    }
                }

                var conference = new Conference
                {
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Location = model.Location,
                    Organizer = model.Organizer,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                    CategoryId = model.CategoryId,
                    BannerImage = fileName
                };

                await _conferenceRepository.AddConferenceAsync(conference);

                TempData["SuccessMessage"] = "Tạo hội nghị thành công!";
                return RedirectToAction("Index");
            }
			var categories = await _categoryRepository.GetAllCategoriesAsync();
			ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // Hiển thị chi tiết hội nghị
        public async Task<IActionResult> ConferenceDetails(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }
            return View(conference);
        }

		// Hiển thị form chỉnh sửa
		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
			if (conference == null)
			{
				return NotFound();
			}

			var viewModel = new ConferenceEditViewModel
			{
				Id = conference.Id,
				Title = conference.Title,
				Description = conference.Description,
				Content = conference.Content,
				StartDate = conference.StartDate,
				EndDate = conference.EndDate,
				Location = conference.Location,
				Organizer = conference.Organizer,
				IsActive = conference.IsActive,
				CategoryId = conference.CategoryId,
				ExistingBannerImage = conference.BannerImage
			};

			var categories = await _categoryRepository.GetAllCategoriesAsync();
			ViewBag.Categories = new SelectList(categories, "Id", "Name", viewModel.CategoryId);

			return View(viewModel);
		}


		// Xử lý chỉnh sửa
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(ConferenceEditViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Load lại danh sách danh mục để hiển thị dropdown nếu có lỗi
				ViewBag.Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
				return View(model);
			}

			var conference = await _conferenceRepository.GetConferenceByIdAsync(model.Id);
			if (conference == null) return NotFound();

			// Cập nhật các trường cơ bản
			conference.Title = model.Title;
			conference.Description = model.Description;
			conference.Content = model.Content;
			conference.StartDate = model.StartDate;
			conference.EndDate = model.EndDate;
			conference.Location = model.Location;
			conference.Organizer = model.Organizer;
			conference.IsActive = model.IsActive;
			conference.CategoryId = model.CategoryId;

			// Xử lý ảnh mới nếu có
			if (model.BannerImage != null && model.BannerImage.Length > 0)
			{
				var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
				var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BannerImage.FileName;
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await model.BannerImage.CopyToAsync(fileStream);
				}

				// Xóa ảnh cũ nếu cần
				if (!string.IsNullOrEmpty(model.ExistingBannerImage))
				{
					var oldPath = Path.Combine(uploadsFolder, model.ExistingBannerImage);
					if (System.IO.File.Exists(oldPath))
					{
						System.IO.File.Delete(oldPath);
					}
				}

				// Cập nhật đường dẫn ảnh mới
				conference.BannerImage = uniqueFileName;
			}

			await _conferenceRepository.UpdateConferenceAsync(conference);
			return RedirectToAction("Index");
		}


        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }
            return View(conference);
        }
    }
}
