namespace HoiNghiKhoaHoc.Repositories
{
    public interface IConferenceCategories
    {
        Task<List<IConferenceCategories>> GetAllCategoriesAsync();
        Task<IConferenceCategories> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(IConferenceCategories category);
        Task UpdateCategoryAsync(IConferenceCategories category);
        Task DeleteCategoryAsync(int id);
    }
}
