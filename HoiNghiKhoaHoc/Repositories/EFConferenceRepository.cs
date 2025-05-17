using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFConferenceRepository : IConferenceRepository
    {
        public Task AddConferenceAsync(Conference conference)
        {
            throw new NotImplementedException();
        }

        public Task DeleteConferenceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Conference>> GetAllConferencesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Conference> GetConferenceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateConferenceAsync(Conference conference)
        {
            throw new NotImplementedException();
        }
    }
}
