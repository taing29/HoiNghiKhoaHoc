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
        private readonly UserManager<ApplicationUser> _userManager;

        public ConferencesController(
            IConferenceRepository conferenceRepo,
            IFavoriteRepository favoriteRepo,
            UserManager<ApplicationUser> userManager)
        {
            _conferenceRepo = conferenceRepo;
            _favoriteRepo = favoriteRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Nếu là Admin, chuyển hướng qua Admin Area
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Conferences", new { area = "Admin" });
            }
            var references = await _conferenceRepo.GetAllConferencesAsync();
            return View(references);
        }

        // Hiển thị danh sách hội nghị công khai
        public async Task<IActionResult> Upcoming()
        {
            var list = await _conferenceRepo.GetUpcomingConferencesAsync();
            return View(list);
        }

        public async Task<IActionResult> Past()
        {
            var list = await _conferenceRepo.GetPastConferencesAsync();
            return View(list);
        }

        public async Task<IActionResult> International()
        {
            var list = await _conferenceRepo.GetInternationalConferencesAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var conference = await _conferenceRepo.GetConferenceByIdAsync(id);
            if (conference == null) return NotFound();

            var related = await _conferenceRepo.GetConferenceByIdCategory(conference);
            var userId = _userManager.GetUserId(User);
            var isFavorite = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var favorite = await _favoriteRepo.GetFavoriteAsync(userId, id);
                isFavorite = favorite != null;
            }

            return View(new ConferenceDetailViewModel
            {
                CurrentConference = conference,
                RelatedConferences = related,
                IsFavorite = isFavorite
            });
        }

        // Dành cho người dùng đăng nhập
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
    }
}