using HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels;
using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Models.ViewModels;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

        public ConferencesController(IConferenceRepository conferenceRepository, ICategoryRepository categoryRepository, IWebHostEnvironment env, ApplicationDbContext context)
        {
            _conferenceRepository = conferenceRepository;
            _categoryRepository = categoryRepository;
            _env = env;
            _context = context;
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
        public async Task<IActionResult> Create(Conference conference, IFormFile imageFile)
        {
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
                TempData["SuccessMessage"] = "Tạo hội nghị thành công.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", conference.CategoryId);
            return View(conference);
        }

        public async Task<IActionResult> ConferenceDetails(int id)
        {
            var conference = await _conferenceRepository.GetConferenceByIdAsync(id);
            if (conference == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hội nghị.";
                return RedirectToAction(nameof(Index));
            }
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
            return View(conference);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Conference conference, IFormFile imageFile)
        {
            ModelState.Remove("imageFile");
            if (id != conference.Id)
            {
                return NotFound();
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
                return View(conference);
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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = registration.ConferenceId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectRegistration(int id)
        {
            var registration = await _context.ConferenceRegistrations.FindAsync(id);
            if (registration != null)
            {
                _context.ConferenceRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
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
    }
}
