using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);
        Task AddFavoriteAsync(Favorite favorite);
        Task RemoveFavoriteAsync(int favoriteId);
        Task<Favorite?> GetFavoriteAsync(string userId, int conferenceId);
    }
}
