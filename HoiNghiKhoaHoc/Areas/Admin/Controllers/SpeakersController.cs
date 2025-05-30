using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpeakersController : Controller
    {
        private readonly ISpeakerRepository _speakerRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        public SpeakersController(ISpeakerRepository speakerRepository, IWebHostEnvironment env, ApplicationDbContext context)
        {
            _speakerRepository = speakerRepository;
            _env = env;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var speakers = await _speakerRepository.GetAllSpeakersAsync();
            return View(speakers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            return View(speaker);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Speaker speaker, IFormFile imageFile)
        {
            ModelState.Remove("imageFile");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }
            try
            {
                if (imageFile != null)
                {
                    speaker.PhotoUrl = await SaveImage(imageFile);
                }
                speaker.CreatedAt = DateTime.Now;
                await _speakerRepository.AddSpeakerAsync(speaker);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo diễn giả: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu diễn giả. Vui lòng thử lại.");
                return View(speaker);
            }
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                var imageFolder = Path.Combine(_env.WebRootPath, "Image");
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var savePath = Path.Combine(imageFolder, fileName);

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return "/Image/" + fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu ảnh: {ex.Message}");
                throw;
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            return View(speaker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Speaker speaker, IFormFile imageUrl)
        {
            ModelState.Remove("imageUrl");
            if (id != speaker.Id) 
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingSpeaker = await _speakerRepository.GetSpeakerByIdAsync(id);
                if(imageUrl == null)
                {
                    speaker.PhotoUrl = existingSpeaker.PhotoUrl;
                }
                else
                {
                    speaker.PhotoUrl = await SaveImage(imageUrl);
                }
                await _speakerRepository.UpdateSpeakerAsync(speaker);
                return RedirectToAction(nameof(Index));
            }
            return View(speaker);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            else
            {
                await _speakerRepository.DeleteSpeakerAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchByName(string name)
        {
            var speakers = await _context.Speakers
                .Where(s => s.FullName.Contains(name))
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Title,
                    s.Affiliation,
                    s.Email,
                    PhotoUrl = Url.Content(s.PhotoUrl ?? "~/Image/default-avatar.jpg")
                })
                .ToListAsync();

            return Json(speakers);
        }
    }
}
