using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
