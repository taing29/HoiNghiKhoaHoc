using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Repositories
{
    public interface ISpeakerRepository
    {
        Task<IEnumerable<Speaker>> GetAllSpeakersAsync();
        Task<Speaker?> GetSpeakerByIdAsync(int id);
        Task AddSpeakerAsync(Speaker speaker);
        Task UpdateSpeakerAsync(Speaker speaker);
        Task DeleteSpeakerAsync(int id);
        Task<IEnumerable<Speaker>> GetSpeakersByConferenceIdAsync(int conferenceId);
    }
}
