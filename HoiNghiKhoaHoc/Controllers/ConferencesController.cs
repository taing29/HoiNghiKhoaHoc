using HoiNghiKhoaHoc.Models.ViewModels;
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

		public async Task<IActionResult> Details(int id)
		{
			var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
			if (conference == null)
			{
				return NotFound();
			}

            var relatedConferences = await _conferenceRepository.GetConferenceByIdCategory(conference);

			var viewModel = new ConferenceDetailViewModel
			{
				CurrentConference = conference,
				RelatedConferences = relatedConferences
			};

			return View(viewModel);
		}
	}
}
