using ITI_Project.Data;
using ITI_Project.DTO;
using ITI_Project.Models;

namespace ITI_Project.Repository
{
    public interface IStockRepository
    {
        Task<Stock> GetStockByProductId(int productId);
        Task ManageStock(StockDTO stockDTO);
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
    }
}
