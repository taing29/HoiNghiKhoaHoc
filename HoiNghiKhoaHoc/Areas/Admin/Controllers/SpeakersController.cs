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
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "Image");
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa có

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    speaker.PhotoUrl = "/Image/" + uniqueFileName; // Lưu đường dẫn tương đối
                }

                speaker.CreatedAt = DateTime.Now;

                await _speakerRepository.AddSpeakerAsync(speaker);
                TempData["SuccessMessage"] = "Thêm diễn giả thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(speaker);
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
