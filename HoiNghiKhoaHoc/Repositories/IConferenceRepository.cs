using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IConferenceRepository
    {
        Task<IEnumerable<Conference>> GetAllConferencesAsync();
        Task<IEnumerable<Conference>> GetAllConferencesPastAsync();
		Task<IEnumerable<Conference>> GetAllConferencesUpcomingAsync();
		Task<IEnumerable<Conference>> GetAllConferencesGlobalAsync();
		Task<IEnumerable<Conference>> GetConferenceByIdCategoryAsync(Conference conference);
		Task<Conference?> GetPastConferenceDetailsByIdAsync(int id);

		Task<Conference> GetConferenceByIdAsync(int id);
		Task AddConferenceAsync(Conference conference);
        Task UpdateConferenceAsync(Conference conference);
        Task DeleteConferenceAsync(int id);
        Task<IEnumerable<Conference>> SearchConferencesAsync(string searchTerm);
    }
}
