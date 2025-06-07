using HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels;
using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
		private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ConferencesController(IConferenceRepository conferenceRepository, ICategoryRepository categoryRepository, 
               IWebHostEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _conferenceRepository = conferenceRepository;
            _categoryRepository = categoryRepository;
            _env = env;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var conferences = await _conferenceRepository.GetAllConferencesAsync();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(conferences);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
			ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conference conference, IFormFile imageFile, List<int> SpeakerIds)
        {
            if (SpeakerIds == null || !SpeakerIds.Any())
                ModelState.AddModelError("SpeakerIds", "Vui lòng chọn ít nhất một diễn giả.");
            if (conference.IsPaid && (conference.Price == null || conference.Price <= 0))
            {
                ModelState.AddModelError("Price", "Vui lòng nhập giá vé hợp lệ.");
            }
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "Image");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    conference.BannerImage = "/Image/" + uniqueFileName;
                }

                conference.CreatedDate = DateTime.Now;
                await _conferenceRepository.AddConferenceAsync(conference);

                // Gán các diễn giả đã chọn
                if (SpeakerIds != null && SpeakerIds.Any())
                {
                    var list = SpeakerIds.Select(id => new ConferenceSpeaker
                    {
                        ConferenceId = conference.Id,
                        SpeakerId = id
                    }).ToList();
                    await _conferenceRepository.AddConferenceSpeakersAsync(list);
                }

                TempData["SuccessMessage"] = "Tạo hội nghị thành công.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
        }

        public async Task<IActionResult> Update(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", conference.CategoryId);

            // Lấy danh sách Speaker đã gán cho hội nghị
            var speakerIds = await _context.ConferenceSpeakers
                .Where(cs => cs.ConferenceId == id)
                .Select(cs => cs.SpeakerId)
                .ToListAsync();

            var speakers = await _context.Speakers
                .Where(s => speakerIds.Contains(s.Id))
                .ToListAsync();

            ViewBag.ExistingSpeakers = speakers;

            return View(conference);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Conference conference, IFormFile imageFile, List<int> SpeakerIds)
        {
            ModelState.Remove("imageFile");
            if (id != conference.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //giữ nguyên thông tin hình ảnh hiện tại
                var existingConference = await _conferenceRepository.GetConferenceByIdAsync(id);
                if (existingConference != null)
                {
                    conference.BannerImage = existingConference.BannerImage;
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "Image");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    conference.BannerImage = "/Image/" + uniqueFileName;
                }
                // Xóa diễn giả cũ trước khi thêm mới
                var existingSpeakers = await _context.ConferenceSpeakers
                    .Where(cs => cs.ConferenceId == conference.Id)
                    .ToListAsync();

                if (existingSpeakers.Any())
                {
                    _context.ConferenceSpeakers.RemoveRange(existingSpeakers);
                    await _context.SaveChangesAsync();
                }

                // Thêm diễn giả mới
                if (SpeakerIds != null && SpeakerIds.Any())
                {
                    var list = SpeakerIds.Select(id => new ConferenceSpeaker
                    {
                        ConferenceId = conference.Id,
                        SpeakerId = id
                    }).ToList();

                    await _conferenceRepository.AddConferenceSpeakersAsync(list);
                }

                await _conferenceRepository.UpdateConferenceAsync(conference);

                TempData["SuccessMessage"] = "Cập nhật hội nghị thành công.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                await _conferenceRepository.DeleteConferenceAsync(id);
                TempData["SuccessMessage"] = "Xóa hội nghị thành công.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                return NotFound();
            }

            var registrations = await _context.ConferenceRegistrations
                .Where(r => r.ConferenceId == id)
                .Include(r => r.User)
                .ToListAsync();

            var viewModel = new ConferenceDetailsViewModel
            {
                Conference = conference,
                Registrations = registrations
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveRegistration(int id)
        {
            var registration = await _context.ConferenceRegistrations.FindAsync(id);
            if (registration != null)
            {
                registration.IsApproved = true;
                registration.IsPaid = true;
                await _context.SaveChangesAsync();
                var user = await _userManager.FindByIdAsync(registration.UserId);
                var conference = await _conferenceRepository.GetConferenceByIdAsync(registration.ConferenceId);

                if (user == null )
                {
                    return NotFound("User không tồn tại.");
                }
                if (conference == null || conference == null)
                {
                    return NotFound("Conference không tồn tại.");
                }

                var emailBody = $@"
				<!DOCTYPE html>
				<html lang=""vi"">
				<head>
				  <meta charset=""UTF-8"">
				  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
				  <style>
					body {{
					  font-family: Arial, sans-serif;
					  background-color: #f6f9fc;
					  margin: 0; padding: 0;
					  color: #333;
					}}
					.container {{
					  max-width: 600px;
					  margin: 20px auto;
					  background: #fff;
					  border-radius: 8px;
					  overflow: hidden;
					  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
					}}
					.banner {{
					  width: 100%;
					  height: 150px;
					  background-image: url('https://yourdomain.com/images/conference-banner.jpg');
					  background-size: cover;
					  background-position: center;
					}}
					.content {{
					  padding: 20px 30px;
					}}
					h2 {{
					  color: #2c3e50;
					  margin-top: 0;
					}}
					p {{
					  font-size: 16px;
					  line-height: 1.5;
					  margin: 10px 0;
					}}
					strong {{
					  color: #2980b9;
					}}
					hr {{
					  border: none;
					  border-top: 1px solid #eee;
					  margin: 20px 0;
					}}
					.footer {{
					  font-size: 14px;
					  color: #999;
					  text-align: center;
					  padding: 10px 20px;
					  background: #f1f1f1;
					}}
				  </style>
				</head>
				<body>
				  <div class=""container"">
					<div class=""banner""></div>
					<div class=""content"">
					  <h2>Xin chào {user.FullName},</h2>
					  <p>Bạn đã đăng ký thành công hội nghị:</p>
					  <p><strong>{conference.Title}</strong></p>
					  <p><strong>Thời gian:</strong> {conference.StartDate:dd/MM/yyyy} đến {conference.EndDate:dd/MM/yyyy}</p>
					  <p><strong>Địa điểm:</strong> {conference.Location}</p>
					  <hr>
					  <p>Cảm ơn bạn đã tham gia và hy vọng sẽ gặp bạn tại hội nghị!</p>
					</div>
					<div class=""footer"">
					  &copy; {DateTime.Now.Year} Hội nghị khoa học - All rights reserved.
					</div>
				  </div>
				</body>
				</html>
				";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Xác nhận đăng ký hội nghị",
                    emailBody
                );
            }
            return RedirectToAction("Details", new { id = registration.ConferenceId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectRegistration(int id)
        {
            var registration = await _context.ConferenceRegistrations.FindAsync(id);
            if (registration != null)
            {
                if(registration.IsApproved)
                {
                    registration.IsApproved = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.ConferenceRegistrations.Remove(registration);
                    await _context.SaveChangesAsync();
                }
                
            }
            return RedirectToAction("Details", new { id = registration.ConferenceId });
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }

            //đổi trạng thái kích hoạt của hội nghị
            conference.IsActive = !conference.IsActive;
            await _conferenceRepository.UpdateConferenceAsync(conference);

            TempData["SuccessMessage"] = "Cập nhật trạng thái hội nghị thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Comment(int id)
        {
            var conference = await _context.Conferences
            .Include(c => c.Comments)
                .ThenInclude(cm => cm.User)
            .FirstOrDefaultAsync(c => c.Id == id);

            if (conference == null)
            {
                return NotFound();
            }

            return View(conference);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bình luận.";
                return RedirectToAction("Index");
            }

            int conferenceId = comment.ConferenceId;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xoá bình luận thành công.";

            return RedirectToAction("Comment", new { id = conferenceId });
        }
    }
}
