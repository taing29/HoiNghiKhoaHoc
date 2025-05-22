using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Controllers
{
    public class ConferencesController : Controller
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly ICategoryRepository _categoryRepository;
		private readonly IConferenceSpeakerRepository _conferenceSpeakerRepository;
		private readonly IConferenceSessionRepository _conferenceSessionRepository;


        public ConferencesController(IConferenceRepository conferenceRepository, ICategoryRepository categoryRepository, IConferenceSpeakerRepository conferenceSpeakerRepository, IConferenceSessionRepository conferenceSessionRepository)
        {
            _conferenceRepository = conferenceRepository;
            _categoryRepository = categoryRepository;
			_conferenceSpeakerRepository = conferenceSpeakerRepository;
			_conferenceSessionRepository = conferenceSessionRepository;
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
		public async Task<IActionResult> Past()
		{
			var conferences = await _conferenceRepository.GetAllConferencesPastAsync();
			return View(conferences);
		}

		public async Task<IActionResult> Upcoming()
		{
			var upcomingConferences = await _conferenceRepository.GetAllConferencesUpcomingAsync();
			return View(upcomingConferences);
		}

		public async Task<IActionResult> Global()
		{
			var globalConferences = await _conferenceRepository.GetAllConferencesGlobalAsync();
			return View(globalConferences);
		}


		public async Task<IActionResult> Details(int id)
		{
			var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
			if (conference == null)
			{
				return NotFound();
			}

			var relatedConferences = await _conferenceRepository.GetConferenceByIdCategoryAsync(conference);
			var speakers = await _conferenceSpeakerRepository.GetSpeakersByConferenceIdAsync(id);
			var sessions = await _conferenceSessionRepository.GetByConferenceId(id);
			var viewModel = new ConferenceDetailViewModel
			{
				CurrentConference = conference,
				RelatedConferences = relatedConferences,
				Speakers = speakers,
				Sessions = sessions
			};

			return View("Details", viewModel);
		}

		public async Task<IActionResult> PastConferenceDetails(int id)
		{
			var conference = await _conferenceRepository.GetPastConferenceDetailsByIdAsync(id);
			Console.WriteLine("Ảnh liên quan: " + conference?.Images?.Count);
			if (conference == null)
			{
				return NotFound();
			}

			var related = await _conferenceRepository.GetConferenceByIdCategoryAsync(conference);

			var viewModel = new ConferenceDetailViewModel
			{
				CurrentConference = conference,
				RelatedConferences = related,
			};

			return View(viewModel);
		}
	}
}
