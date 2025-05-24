using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net; 


namespace HoiNghiKhoaHoc.Controllers
{
    public class ConferencesController : Controller
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConferenceSpeakerRepository _conferenceSpeakerRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ConferencesController(
            IConferenceRepository conferenceRepository,
            IFavoriteRepository favoriteRepository,
            IRegistrationRepository registrationRepository,
            UserManager<ApplicationUser> userManager,
            ICategoryRepository categoryRepository,
            IConferenceSpeakerRepository conferenceSpeakerRepository
            )
        {
            _conferenceRepository = conferenceRepository;
            _favoriteRepository = favoriteRepository;
            _registrationRepository = registrationRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _conferenceSpeakerRepository = conferenceSpeakerRepository;
        }
        public async Task<IActionResult> Index(string? searchString)
        {
            // Nếu là Admin, chuyển hướng sang Area Admin
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Conferences", new { area = "Admin" });
            }
            var results = await _conferenceRepository.SearchConferencesAsync(searchString ?? "");
            var favoriteIds = new List<int>();
            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                var userId = _userManager.GetUserId(User);
                var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
                favoriteIds = favorites.Select(f => f.ConferenceId).ToList();
            }

            ViewBag.FavoriteIds = favoriteIds;
            ViewBag.Search = searchString;
            return View(results);

          
        }

        public async Task<IActionResult> Upcoming()
        {
            var list = await _conferenceRepository.GetAllConferencesUpcomingAsync();
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
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null) return NotFound();
            //decode html 
			conference.Content = WebUtility.HtmlDecode(conference.Content);

			var related = await _conferenceRepository.GetConferenceByIdCategoryAsync(conference);
            var speakers = await _conferenceSpeakerRepository.GetSpeakersByConferenceIdAsync(id);
            var userId = _userManager.GetUserId(User);
            var isFavorite = false;
            var isRegistered = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var favorite = await _favoriteRepository.GetFavoriteAsync(userId, id);
                isFavorite = favorite != null;

                var registration = await _registrationRepository.GetRegistrationAsync(userId, id);
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
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
            return View(favorites);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _favoriteRepository.GetFavoriteAsync(userId, conferenceId);
            if (existing == null)
            {
                await _favoriteRepository.AddFavoriteAsync(new Favorite
                {
                    UserId = userId,
                    ConferenceId = conferenceId,
                    DateAdded = DateTime.Now
                });
            }
            //kiểm tra đang ở trang nào để redirect về đúng trang
            var referer = Request.Headers["Referer"].ToString();
            var conferenceIdFromReferer = referer.Split('/').LastOrDefault();
            if (int.TryParse(conferenceIdFromReferer, out int conferenceIdFromRefererInt) && conferenceIdFromRefererInt == conferenceId)
            {
                //thêm thông báo thành công
                TempData["SuccessMessage"] = "Đã thêm vào yêu thích thành công!";
                return Redirect(referer); // Redirect back to the same page
            }
            else
            {
                return RedirectToAction("Index", new { id = conferenceId });
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var favorite = await _favoriteRepository.GetFavoriteAsync(userId, conferenceId);
            if (favorite != null)
            {
                await _favoriteRepository.RemoveFavoriteAsync(favorite.Id);
            }
            var referer = Request.Headers["Referer"].ToString();
            var conferenceIdFromReferer = referer.Split('/').LastOrDefault();
            if (int.TryParse(conferenceIdFromReferer, out int conferenceIdFromRefererInt) && conferenceIdFromRefererInt == conferenceId)
            {
                //thêm thông báo thành công
                TempData["SuccessMessage"] = "Đã xóa khỏi danh sách yêu thích.";
                return Redirect(referer); // Redirect back to the same page
            }
            else
            {
                return RedirectToAction("Details", new { id = conferenceId });
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyRegistrations()
        {
            var userId = _userManager.GetUserId(User);
            var registrations = await _registrationRepository.GetRegistrationsByUserIdAsync(userId);
            return View(registrations);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Register(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
            if (existing == null)
            {
                await _registrationRepository.RegisterAsync(new ConferenceRegistration
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
        //    var existing = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
        //    if (existing == null)
        //    {
        //        await _registrationRepository.RegisterAsync(new ConferenceRegistration
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
        //    var registration = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);

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
            var reg = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
            if (reg != null)
            {
                await _registrationRepository.CancelAsync(reg.Id);
            }
            return RedirectToAction("Details", new { id = conferenceId });
        }
    }
}
