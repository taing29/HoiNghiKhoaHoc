namespace HoiNghiKhoaHoc.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<ICategoryRepository>> GetAllCategoriesAsync();
        Task<ICategoryRepository> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(ICategoryRepository category);
        Task UpdateCategoryAsync(ICategoryRepository category);
        Task DeleteCategoryAsync(int id);
    }
}
