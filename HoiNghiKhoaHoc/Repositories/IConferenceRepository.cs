using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IConferenceRepository
    {
        Task<IEnumerable<Conference>> GetAllConferencesAsync();
        Task<Conference> GetConferenceByIdAsync(int id);
        Task AddConferenceAsync(Conference conference);
        Task UpdateConferenceAsync(Conference conference);
        Task DeleteConferenceAsync(int id);
        Task<Conference> DisplayConferenceAsync(int id);

    }
}
