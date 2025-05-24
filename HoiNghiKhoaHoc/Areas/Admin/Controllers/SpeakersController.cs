using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpeakersController : Controller
    {
        private readonly ISpeakerRepository _speakerRepository;
        private readonly IWebHostEnvironment _env;
        public SpeakersController(ISpeakerRepository speakerRepository, IWebHostEnvironment env)
        {
            _speakerRepository = speakerRepository;
            _env = env;
        }

        // GET: Admin/Speakers
        public async Task<IActionResult> Index()
        {
            var speakers = await _speakerRepository.GetAllSpeakersAsync();
            return View(speakers);
        }

        // GET: Admin/Speakers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            return View(speaker);
        }

        // GET: Admin/Speakers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Speakers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Speaker speaker, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine($"ModelState errors: {string.Join("; ", errors)}");
                return View(speaker);
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

        // GET: Admin/Speakers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            return View(speaker);
        }

        // POST: Admin/Speakers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Speaker speaker)
        {
            if (id != speaker.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _speakerRepository.UpdateSpeakerAsync(speaker);
                return RedirectToAction(nameof(Index));
            }
            return View(speaker);
        }

        // GET: Admin/Speakers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var speaker = await _speakerRepository.GetSpeakerByIdAsync(id);
            if (speaker == null) return NotFound();
            return View(speaker);
        }

        // POST: Admin/Speakers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _speakerRepository.DeleteSpeakerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
