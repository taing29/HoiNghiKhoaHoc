using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IConferencesRepository
    {
        Task<List<Conferences>> GetAllConferencesAsync();
        Task<Conferences> GetConferenceByIdAsync(int id);
        Task AddConferenceAsync(Conferences conference);
        Task UpdateConferenceAsync(Conferences conference);
        Task DeleteConferenceAsync(int id);
    }
}
