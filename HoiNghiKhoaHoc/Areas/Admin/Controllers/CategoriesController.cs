using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddCategoryAsync(category);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateCategoryAsync(category);
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var categories = await _categoryRepository.GetCategoryByIdAsync(id);
            if (categories == null) return NotFound();
            else
            {
                await _categoryRepository.DeleteCategoryAsync(id);
                return RedirectToAction("Index");
            }
        }
    }
}
