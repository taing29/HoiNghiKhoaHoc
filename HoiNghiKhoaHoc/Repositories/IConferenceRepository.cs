using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IConferenceRepository
    {
        Task<IEnumerable<Conference>> GetAllConferencesAsync();
		Task<IEnumerable<Conference>> GetConferenceByIdCategory(Conference conference);
		Task<Conference> GetConferenceByIdAsync(int id);
		Task AddConferenceAsync(Conference conference);
        Task UpdateConferenceAsync(Conference conference);
        Task DeleteConferenceAsync(int id);
    }
}
