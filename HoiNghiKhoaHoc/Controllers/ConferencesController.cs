using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Controllers
{
    public class ConferencesController : Controller
    {
        private readonly IConferenceRepository _conferenceRepo;
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly IRegistrationRepository _registrationRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConferenceSpeakerRepository _conferenceSpeakerRepository
        private readonly ICategoryRepository _categoryRepository

        public ConferencesController(
            IConferenceRepository conferenceRepo,
            IFavoriteRepository favoriteRepo,
            IRegistrationRepository registrationRepo,
            UserManager<ApplicationUser> userManager,
            ICategoryRepository categoryRepository,
            IConferenceSpeakerRepository conferenceSpeakerRepository
            )
        {
            _conferenceRepo = conferenceRepo;
            _categoryRepository = categoryRepository;
            _conferenceSpeakerRepository = conferenceSpeakerRepository;
            _favoriteRepo = favoriteRepo;
            _registrationRepo = registrationRepo;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string? searchString)
        {
            // Nếu là Admin, chuyển hướng sang Area Admin
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Conferences", new { area = "Admin" });
            }
            var results = await _conferenceRepo.SearchConferencesAsync(searchString ?? "");
            var favoriteIds = new List<int>();
            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                var userId = _userManager.GetUserId(User);
                var favorites = await _favoriteRepo.GetFavoritesByUserIdAsync(userId);
                favoriteIds = favorites.Select(f => f.ConferenceId).ToList();
            }

            ViewBag.FavoriteIds = favoriteIds;
            ViewBag.Search = searchString;
            return View(results);

          
        }




        public async Task<IActionResult> Upcoming()
        {
            var list = await _conferenceRepo.GetUpcomingConferencesAsync();
            return View(list);
        }
		public async Task<IActionResult> Past()
		{
			var conferences = await _conferenceRepository.GetAllConferencesPastAsync();
			return View(conferences);
		}
        public async Task<IActionResult> Global()
        {
            var globalConferences = await _conferenceRepository.GetAllConferencesGlobalAsync();
            return View(globalConferences);
        }


        public async Task<IActionResult> Details(int id)
        {
            var conference = await _conferenceRepo.GetConferenceByIdAsync(id);
            if (conference == null) return NotFound();

            var related = await _conferenceRepo.GetConferenceByIdCategory(conference);
            var speakers = await _conferenceSpeakerRepository.GetSpeakersByConferenceIdAsync(id);
            var userId = _userManager.GetUserId(User);
            var isFavorite = false;
            var isRegistered = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var favorite = await _favoriteRepo.GetFavoriteAsync(userId, id);
                isFavorite = favorite != null;

                var registration = await _registrationRepo.GetRegistrationAsync(userId, id);
                isRegistered = registration != null;
            }

            return View(new ConferenceDetailViewModel
            {
                CurrentConference = conference,
                RelatedConferences = related,
                Speakers = speakers,
                IsFavorite = isFavorite,
                IsRegistered = isRegistered
            });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyFavorites()
        {
            var userId = _userManager.GetUserId(User);
            var favorites = await _favoriteRepo.GetFavoritesByUserIdAsync(userId);
            return View(favorites);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _favoriteRepo.GetFavoriteAsync(userId, conferenceId);
            if (existing == null)
            {
                await _favoriteRepo.AddFavoriteAsync(new Favorite
                {
                    UserId = userId,
                    ConferenceId = conferenceId,
                    DateAdded = DateTime.Now
                });
            }
            return RedirectToAction("Details", new { id = conferenceId });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var favorite = await _favoriteRepo.GetFavoriteAsync(userId, conferenceId);
            if (favorite != null)
            {
                await _favoriteRepo.RemoveFavoriteAsync(favorite.Id);
            }
            return RedirectToAction("Details", new { id = conferenceId });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyRegistrations()
        {
            var userId = _userManager.GetUserId(User);
            var registrations = await _registrationRepo.GetRegistrationsByUserIdAsync(userId);
            return View(registrations);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Register(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _registrationRepo.GetRegistrationAsync(userId, conferenceId);
            if (existing == null)
            {
                await _registrationRepo.RegisterAsync(new ConferenceRegistration
                {
                    UserId = userId,
                    ConferenceId = conferenceId,
                    RegisteredDate = DateTime.Now
                });
            }
            return RedirectToAction("Details", new { id = conferenceId });
        }






        //[Authorize(Roles = "User")]
        //[HttpPost]
        //public async Task<IActionResult> Registerr(int conferenceId)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var existing = await _registrationRepo.GetRegistrationAsync(userId, conferenceId);
        //    if (existing == null)
        //    {
        //        await _registrationRepo.RegisterAsync(new ConferenceRegistration
        //        {
        //            UserId = userId,
        //            ConferenceId = conferenceId,
        //            RegisteredDate = DateTime.Now
        //        });
        //    }

           
        //    return RedirectToAction("RegistrationConfirmation", new { conferenceId });
        //}
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> RegistrationConfirmation(int conferenceId)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var registration = await _registrationRepo.GetRegistrationAsync(userId, conferenceId);

        //    if (registration == null)
        //        return RedirectToAction("Details", new { id = conferenceId }); 

        //    var conference = await _conferenceRepo.GetConferenceByIdAsync(conferenceId);
        //    if (conference == null) return NotFound();

        //    ViewBag.RegisteredDate = registration.RegisteredDate;
        //    return View(conference); 
        //}



        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CancelRegistration(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var reg = await _registrationRepo.GetRegistrationAsync(userId, conferenceId);
            if (reg != null)
            {
                await _registrationRepo.CancelAsync(reg.Id);
            }
            return RedirectToAction("Details", new { id = conferenceId });
        }
    }
}
