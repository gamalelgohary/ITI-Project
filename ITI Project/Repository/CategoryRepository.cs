using Humanizer.Localisation;
using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Repository
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly ApplicationDbContext context;
        public CategoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddCategory(Category category)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategory(Category category)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(g => g.Id == id);
            return category;
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            context.Categories.Update(category);
            await context.SaveChangesAsync();
        }
    }
}
