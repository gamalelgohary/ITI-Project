using ITI_Project.DTO;
using ITI_Project.Models;

namespace ITI_Project.Repository
{
    public interface ICartRepository
    {
        Task<int> AddItem(int productId, int Qty);
        Task<int> RemoveItem(int productId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId);
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(CheckoutModel model);
    }
}

