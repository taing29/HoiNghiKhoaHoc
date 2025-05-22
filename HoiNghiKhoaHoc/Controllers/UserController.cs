using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetAllUsersAsync();
			return View(users);
		}

		public async Task<IActionResult> Edit(string id)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null) return NotFound();

			var model = new UserView
			{
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(UserView model)
		{
			var user = await _userRepository.GetByIdAsync(model.Id);
			if (user == null) return NotFound();

			user.FullName = model.FullName;
			user.Email = model.Email;
			user.PhoneNumber = model.PhoneNumber;

			await _userRepository.UpdateUserAsync(user);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			await _userRepository.DeleteUserAsync(id);
			return RedirectToAction("Index");
		}
	}
}
