
using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFFavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public EFFavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
        {
            return await _context.Favorites
                .Include(f => f.Conference)
                .ThenInclude(c => c.Category)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int favoriteId)
        {
            var favorite = await _context.Favorites.FindAsync(favoriteId);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Favorite?> GetFavoriteAsync(string userId, int conferenceId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ConferenceId == conferenceId);
        }
    }
}
