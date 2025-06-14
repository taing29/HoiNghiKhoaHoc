﻿using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rotativa.AspNetCore;  // thêm để sinh PDF
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
		private readonly ICommentRepository _commentRepository;
		private readonly IEmailSender _emailSender;

		public ConferencesController(
			IConferenceRepository conferenceRepository,
			IFavoriteRepository favoriteRepository,
			IRegistrationRepository registrationRepository,
			UserManager<ApplicationUser> userManager,
			ICategoryRepository categoryRepository,
			IConferenceSpeakerRepository conferenceSpeakerRepository,
			ICommentRepository commentRepository,
			IEmailSender emailSender
			)
		{
			_conferenceRepository = conferenceRepository;
			_favoriteRepository = favoriteRepository;
			_registrationRepository = registrationRepository;
			_userManager = userManager;
			_categoryRepository = categoryRepository;
			_conferenceSpeakerRepository = conferenceSpeakerRepository;
			_commentRepository = commentRepository;
			_emailSender = emailSender;
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
			var comments = await _commentRepository.GetCommentsByConferenceIdAsync(id);
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
				IsRegistered = isRegistered,
				Comments = comments
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
			var conference = await _conferenceRepository.GetConferenceByIdAsync(conferenceId);
			if (conference == null)
				return RedirectToAction("Details", new { id = conferenceId });
			if (existing == null)
			{
				await _registrationRepository.RegisterAsync(new ConferenceRegistration
				{
					UserId = userId,
					ConferenceId = conferenceId,
					RegisteredDate = DateTime.Now,
					IsPaid = !conference.IsPaid
				});

				// Gửi email xác nhận sau khi đăng ký thành công
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					return RedirectToAction("Login", "Account");

				if (conference.IsPaid)
				{
					// Điều hướng tới trang thanh toán
					return RedirectToAction("Pay", new { conferenceId });
				}
			}
			return RedirectToAction("RegistrationConfirmation", new { conferenceId });
		}


		[Authorize(Roles = "User")]
		public async Task<IActionResult> RegistrationConfirmation(int conferenceId)
		{
			var userId = _userManager.GetUserId(User);
			var registration = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
			if (registration == null)
				return RedirectToAction("Details", new { id = conferenceId });

			var conference = await _conferenceRepository.GetConferenceByIdAsync(conferenceId);
			if (conference == null) return NotFound();

			var user = await _userManager.FindByIdAsync(userId);

			ViewBag.RegisteredDate = registration.RegisteredDate;
			ViewBag.IsApproved = registration.IsApproved;

			ViewBag.UserFullName = user?.FullName ?? "";
			ViewBag.UserEmail = user?.Email ?? "";
			ViewBag.UserPhoneNumber = user?.PhoneNumber ?? "";

			return View(conference);
		}

        [Authorize(Roles = "User")]
        public async Task<IActionResult> DownloadConfirmation(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
            if (registration == null)
                return RedirectToAction("Details", new { id = conferenceId });

            var conference = await _conferenceRepository.GetConferenceByIdAsync(conferenceId);
            if (conference == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);

            var vm = new ConferenceRegistrationView
            {
                ConferenceId = conferenceId,
                FullName = user?.FullName ,
                Email = user?.Email ,
                PhoneNumber = user?.PhoneNumber,
                Conference = conference
            };
            ViewBag.RegisteredDate = registration.RegisteredDate;
            return View("DownloadConfirmation", vm);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> DownloadPdf(int conferenceId)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _registrationRepository
                                    .GetRegistrationAsync(userId, conferenceId);
            if (registration == null)
                return RedirectToAction("Details", new { id = conferenceId });

            var conference = await _conferenceRepository
                                    .GetConferenceByIdAsync(conferenceId);
            if (conference == null) return NotFound();

            // Gán lại ViewBag.RegisteredDate để view có dữ liệu
            ViewBag.RegisteredDate = registration.RegisteredDate;

            var user = await _userManager.FindByIdAsync(userId);

            var vm = new ConferenceRegistrationView
            {
                ConferenceId = conferenceId,
                FullName = user?.FullName,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber,
                Conference = conference
            };

            // Đánh dấu PDF (nếu bạn đang dùng flag để ẩn footer)
            ViewBag.IsPdf = true;

            return new ViewAsPdf("DownloadConfirmation", vm)
            {
                FileName = $"GiayXacNhan_{conferenceId}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }


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

		public async Task<IActionResult> AddComment(AddCommentViewModel model)
		{
			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Nội dung bình luận không được để trống.";
				return RedirectToAction("Details", new { id = model.ConferenceId });
			}

			if (User.Identity.IsAuthenticated)
			{
				var userId = _userManager.GetUserId(User);
				var comment = new Comment
				{
					ConferenceId = model.ConferenceId,
					UserId = userId,
					Content = model.Content,
					CreatedAt = DateTime.Now
				};
				await _commentRepository.AddCommentAsync(comment);
				TempData["Message"] = "Bình luận đã được gửi thành công!";
			}
			return RedirectToAction("Details", new { id = model.ConferenceId });
		}

		[Authorize(Roles = "User")]
		public async Task<IActionResult> Pay(int conferenceId)
		{
			var regJson = TempData["RegistrationInfo"] as string;
			if (regJson == null)
				return RedirectToAction("ConferenceDetails", new { id = conferenceId });

			var registration = JsonConvert.DeserializeObject<ConferenceRegistrationView>(regJson);
			var conference = await _conferenceRepository.GetConferenceByIdAsync(conferenceId);

			if (conference == null)
				return RedirectToAction("Details", new { id = conferenceId });

			ViewBag.Registration = registration;
			TempData["RegistrationInfo"] = regJson; 
			return View(conference);
		}


		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> ConfirmPayment(int conferenceId)
		{
			var userId = _userManager.GetUserId(User);
			var regJson = TempData["RegistrationInfo"] as string;
			if (string.IsNullOrEmpty(regJson))
				return RedirectToAction("RegisterInfo", new { conferenceId });

			var registrationView = JsonConvert.DeserializeObject<ConferenceRegistrationView>(regJson);

			var existing = await _registrationRepository.GetRegistrationAsync(userId, conferenceId);
			if (existing == null)
			{
				await _registrationRepository.RegisterAsync(new ConferenceRegistration
				{
					UserId = userId,
					ConferenceId = conferenceId,
					RegisteredDate = DateTime.UtcNow,
					IsPaid = true,
					PaidDate = DateTime.UtcNow
				});
			}
			else
			{
				if (existing.IsPaid)
					return RedirectToAction("RegistrationConfirmation", new { conferenceId });

				existing.IsPaid = true;
				existing.PaidDate = DateTime.UtcNow;
				await _registrationRepository.UpdateAsync(existing);
			}
			return RedirectToAction("RegistrationConfirmation", new { conferenceId });
		}



		// Hiển thị form nhập thông tin trước thanh toán
		[HttpGet]
		public IActionResult RegisterInfo(int conferenceId)
		{
			var email = User.Identity?.IsAuthenticated == true
				? _userManager.GetUserAsync(User).Result?.Email ?? ""
				: "";
			var model = new ConferenceRegistrationView
			{
				ConferenceId = conferenceId,
				Email = email
			};
			return View(model);
		}

		//Gửi form và chuyển đến trang thanh toán
		[HttpPost]
		public async Task<IActionResult> RegisterInfo(ConferenceRegistrationView model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var conference = await _conferenceRepository.GetConferenceByIdAsync(model.ConferenceId);
			if (conference == null) return RedirectToAction("Details", new { id = model.ConferenceId });

			// Lấy user hiện tại
			var user = await _userManager.GetUserAsync(User);

			if (user != null)
			{
				// Nếu số điện thoại trong user khác số điện thoại nhập hoặc chưa có thì cập nhật
				if (string.IsNullOrEmpty(user.PhoneNumber))
				{
					user.PhoneNumber = model.PhoneNumber;
					var result = await _userManager.UpdateAsync(user);
					if (!result.Succeeded)
					{
						// Xử lý lỗi cập nhật (có thể log hoặc thêm ModelState error)
						ModelState.AddModelError("", "Không thể cập nhật số điện thoại.");
						return View(model);
					}
				}
			}

			// Lưu thông tin tạm vào TempData
			TempData["RegistrationInfo"] = JsonConvert.SerializeObject(model);

			if (!conference.IsPaid)
			{
				// Nếu hội nghị miễn phí thì đăng ký luôn
				// Thực hiện logic giống Register ở đây
				var userId = _userManager.GetUserId(User);
				var existing = await _registrationRepository.GetRegistrationAsync(userId, model.ConferenceId);
				if (existing == null)
				{
					await _registrationRepository.RegisterAsync(new ConferenceRegistration
					{
						UserId = userId,
						ConferenceId = model.ConferenceId,
						RegisteredDate = DateTime.Now,
						IsPaid = true
					});
				}

				return RedirectToAction("RegistrationConfirmation", new { conferenceId = model.ConferenceId });
			}


			// Nếu hội nghị có phí thì chuyển đến trang thanh toán
			return RedirectToAction("Pay", new { conferenceId = model.ConferenceId });
		}




	}
}
