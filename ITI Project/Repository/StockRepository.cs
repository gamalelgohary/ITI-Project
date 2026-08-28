using ITI_Project.Data;
using ITI_Project.DTO;
using ITI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext context;

        public StockRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Stock?> GetStockByProductId(int productId)
        {
            var stock = await context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
            return stock;
        }

        public async Task ManageStock(StockDTO stockDTO)
        {
            var exitingStock = await GetStockByProductId(stockDTO.ProductId);
            if (exitingStock is null)
            {
                var stock = new Stock
                {
                    ProductId = stockDTO.ProductId,
                    Quantity = stockDTO.Quantity
                };
                context.Stocks.Add(stock);
            }
            else
            {
                exitingStock.Quantity = stockDTO.Quantity;
            }
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from product in context.Products
                                join stock in context.Stocks
                                on product.Id equals stock.ProductId
                                into product_stock
                                from productStock in product_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || product.ProductName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    ProductId = product.Id,
                                    ProductName = product.ProductName,
                                    Quantity = productStock == null ? 0 : productStock.Quantity
                                }
                               ).ToListAsync();
            return stocks;
        }
    }
}
