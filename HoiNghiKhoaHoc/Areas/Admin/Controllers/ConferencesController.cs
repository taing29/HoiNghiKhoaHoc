using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            _conferenceRepository = conferenceRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var refences = await _conferenceRepository.GetAllConferencesAsync();
            return View(refences);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conference conference)
        {
            if (ModelState.IsValid)
            {
                conference.CreatedDate = DateTime.Now;
                await _conferenceRepository.AddConferenceAsync(conference);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
        }

    }
}
