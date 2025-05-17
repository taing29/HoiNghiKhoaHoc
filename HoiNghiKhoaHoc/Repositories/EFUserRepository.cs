using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
	public class EFUserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;
		public EFUserRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public Task<ApplicationUser> AddUserAsync(ApplicationUser user)
		{
			_context.Users.Add(user);
			throw new NotImplementedException();
		}

		public Task<ApplicationUser> DeleteUserAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			throw new NotImplementedException();
		}

		public Task<ApplicationUser> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
		{
			_context.Update(user);
			throw new NotImplementedException();
		}
	}
}
