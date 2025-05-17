using HoiNghiKhoaHoc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFUserRepository : IUserRepository
	{
		private readonly UserManager<ApplicationUser> _userManager; 
		public EFUserRepository(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			return await _userManager.Users.ToListAsync();
		}

		public async Task<ApplicationUser> GetByIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}

		public async Task AddUserAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateUserAsync(ApplicationUser user)
		{
			await _userManager.UpdateAsync(user); // OK
		}

		public async Task DeleteUserAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}
		}
	}
}
