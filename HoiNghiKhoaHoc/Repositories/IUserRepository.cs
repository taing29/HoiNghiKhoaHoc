using HoiNghiKhoaHoc.Models;
namespace HoiNghiKhoaHoc.Repositories
{
	public interface IUserRepository
	{
		Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
		Task<ApplicationUser> GetByIdAsync(string id);
		Task AddUserAsync(ApplicationUser user);
		Task UpdateUserAsync(ApplicationUser user);
		Task DeleteUserAsync(string id);

	}
}
