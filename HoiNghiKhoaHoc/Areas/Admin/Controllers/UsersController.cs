using HoiNghiKhoaHoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public UsersController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		// GET: /Admin/Users
		public IActionResult Index()
		{
			var users = _userManager.Users.ToList();

			var userViews = users.Select(u => new UserView
			{
				UserName = u.UserName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				FullName = u.FullName,
				Age = u.Age
			}).ToList();

			return View(userViews);
		}

		// GET: /Admin/EditUser/id
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			var model = new UserView
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Age = user.Age,
				UserName = user.UserName
			};

			return View(model);
		}

		// POST: /Admin/EditUser
		[HttpPost]
		public async Task<IActionResult> EditUser(UserView model)
		{
			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null) return NotFound();

			user.FullName = model.FullName;
			user.Email = model.Email;
			user.PhoneNumber = model.PhoneNumber;

			await _userManager.UpdateAsync(user);

			return RedirectToAction("Users");
		}

		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			var model = new UserView
			{
				UserName = user.UserName,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Age = user.Age
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}
			return RedirectToAction("Index");
		}
	}
}
