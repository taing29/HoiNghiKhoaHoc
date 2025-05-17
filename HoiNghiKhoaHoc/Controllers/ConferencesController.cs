using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Controllers
{
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
            // Nếu là Admin, chuyển hướng qua Admin Area
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Conferences", new { area = "Admin" });
            }
            var references = await _conferenceRepository.GetAllConferencesAsync();
            return View(references);
        }
    }
}
