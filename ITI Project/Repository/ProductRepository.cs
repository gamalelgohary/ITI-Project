using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                                 .Include(p => p.Categories)   // لو عايز تجيب الكاتيجوري كمان
                                 .Include(p => p.Stock)      // لو عايز تجيب الستوك
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products
                                 .Include(p => p.Categories)
                                 .Include(p => p.Stock)
                                 .ToListAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Product>> DisplayProducts(string sTerm = "", int CategoryId = 0)
        {
            var productQuery = _context.Products
               .AsNoTracking()
               .Include(x => x.Categories)
               .Include(x => x.Stock)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                productQuery = productQuery.Where(b => b.ProductName.StartsWith(sTerm.ToLower()));
            }

            if (CategoryId > 0)
            {
                productQuery = productQuery.Where(b => b.CategoryId == CategoryId);
            }

            var product = await productQuery
                .AsNoTracking()
                .Select(product => new Product
                {
                    Id = product.Id,
                    Image = product.Image,
                    Description = product.Description,
                    ProductName = product.ProductName,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    CategoryName = product.Categories.CategoryName,
                    Quantity = product.Stock == null ? 0 : product.Stock.Quantity
                }).ToListAsync();

            return product;
        }


    }
}
