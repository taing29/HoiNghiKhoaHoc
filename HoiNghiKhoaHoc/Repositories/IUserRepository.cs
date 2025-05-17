using HoiNghiKhoaHoc.Models;
namespace HoiNghiKhoaHoc.Repositories
{
	public interface IUserRepository
	{
		Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
		Task<ApplicationUser> GetByIdAsync(int id);
		Task<ApplicationUser> AddUserAsync(ApplicationUser user);
		Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
		Task<ApplicationUser> DeleteUserAsync(int id);
		
	}
}
