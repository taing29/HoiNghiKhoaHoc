using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public ConferencesController(IConferenceRepository conferenceRepository, ICategoryRepository categoryRepository)
        {
            _conferenceRepository = conferenceRepository ?? throw new ArgumentNullException(nameof(conferenceRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
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
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conference conference)
        {
            if (ModelState.IsValid)
            {
                conference.CreatedDate = DateTime.Now; // 05:29 AM +07, May 18, 2025
                await _conferenceRepository.AddConferenceAsync(conference);
                TempData["SuccessMessage"] = "Hội nghị đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
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
        public async Task<IActionResult> Update(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
        }

        // Xử lý chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Conference conference)
        {
            if (id != conference.Id)
            {
                TempData["ErrorMessage"] = "ID hội nghị không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _conferenceRepository.UpdateConferenceAsync(conference);
                    TempData["SuccessMessage"] = "Hội nghị đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật hội nghị.";
                }
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
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

        // Xử lý xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _conferenceRepository.DeleteConferenceAsync(id);
                TempData["SuccessMessage"] = "Hội nghị đã được xóa thành công!";
             
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hội nghị.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}