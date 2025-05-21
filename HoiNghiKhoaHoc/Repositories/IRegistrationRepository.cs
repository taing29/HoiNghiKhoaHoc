using HoiNghiKhoaHoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface IRegistrationRepository
    {
        Task<ConferenceRegistration> GetRegistrationAsync(string userId, int conferenceId);
        Task RegisterAsync(ConferenceRegistration registration);
        Task CancelAsync(int registrationId);
        Task<IEnumerable<ConferenceRegistration>> GetRegistrationsByUserIdAsync(string userId);
    }
}
