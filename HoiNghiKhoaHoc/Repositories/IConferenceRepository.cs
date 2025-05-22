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

        Task<IEnumerable<Conference>> GetUpcomingConferencesAsync(); 
        Task<IEnumerable<Conference>> GetPastConferencesAsync();    
        Task<IEnumerable<Conference>> GetInternationalConferencesAsync();
        Task<IEnumerable<Conference>> SearchConferencesAsync(string searchTerm);



    }
}
