using Humanizer.Localisation;
using ITI_Project.Models;

namespace ITI_Project.Repository
{
    public interface ICategoryRepository
    {
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task<Category?> GetCategoryById(int id);
        Task DeleteCategory(Category category);
        Task<IEnumerable<Category>> GetCategory();
    }
}
