using ITI_Project.Data;
using ITI_Project.DTO;
using ITI_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ITI_Project.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartRepository(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int productId, int Qty)
        {
            string userId = GetUserId();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");//login مش عامل

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                    };
                    context.ShoppingCarts.Add(cart);
                }
                context.SaveChanges();

                var CartDetail = context.CartDetails.FirstOrDefault(c => c.ShoppingCart_Id == cart.Id && c.ProductId == productId);
                if (CartDetail is not null)
                {
                    CartDetail.Quantity += Qty;
                }
                else
                {
                    var product = context.Products.Find(productId);
                    CartDetail = new CartDetail
                    {
                        ProductId = productId,
                        Quantity = Qty,
                        ShoppingCart_Id = cart.Id,
                        UnitPrice = product.Price
                    };

                    context.CartDetails.Add(CartDetail);
                }
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int productId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");//login مش عامل

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new InvalidOperationException("Invalid Cart");
                }

                var CartDetail = context.CartDetails.FirstOrDefault(c => c.ShoppingCart_Id == cart.Id && c.ProductId == productId);
                if (CartDetail is null)
                    throw new InvalidOperationException("No Item In The Cart");
                else if (CartDetail.Quantity == 1)
                    context.CartDetails.Remove(CartDetail);
                else
                    CartDetail.Quantity--;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid UserId");

            var ShoppingCart = await context.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Product)
                                  .ThenInclude(a => a.Stock)
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Product)
                                  .ThenInclude(a => a.Categories)
                                  .Where(Sh => Sh.UserId == userId).FirstOrDefaultAsync();

            return ShoppingCart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in context.ShoppingCarts
                              join cartDetail in context.CartDetails
                              on cart.Id equals cartDetail.ShoppingCart_Id
                              where cart.UserId == userId
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            ShoppingCart cart = await context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");

                var cartDetail = context.CartDetails
                                            .Where(a => a.ShoppingCart_Id == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Cart is empty");

                var pendingRecord = context.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new InvalidOperationException("Order status does not have Pending status");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = pendingRecord.Id,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false
                };
                context.Orders.Add(order);
                context.SaveChanges();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    context.OrderDetails.Add(orderDetail);

                    var stock = await context.Stocks.FirstOrDefaultAsync(a => a.ProductId == item.ProductId);
                    if (stock == null)
                    {
                        throw new InvalidOperationException("Stock is null");
                    }

                    if (item.Quantity > stock.Quantity)
                    {
                        throw new InvalidOperationException($"Only {stock.Quantity} items(s) are available in the stock");
                    }

                    stock.Quantity -= item.Quantity;
                }

                context.CartDetails.RemoveRange(cartDetail);
                context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var user = httpContextAccessor.HttpContext.User;
            var userId = userManager.GetUserId(user);
            return userId;
        }
    }
}