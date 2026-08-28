using ITI_Project.Data;
using ITI_Project.DTO;
using ITI_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Repository
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;

        public UserOrderRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task ChangeOrderStatus(UpdateOrderStatusModel data)
        {
            var order = await context.Orders.FindAsync(data.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order with id:{data.OrderId} does not found");
            }
            order.OrderStatusId = data.OrderStatusId;
            await context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await context.OrderStatuses.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order withi id:{orderId} does not found");
            }
            order.IsPaid = !order.IsPaid;
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
        {
            var orders = context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)   
                .ThenInclude(o => o.Categories).AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged-in");

                orders = orders.Where(a => a.UserId == userId);
                return await orders.ToListAsync();
            }

            return await orders.ToListAsync();
        }

        private string GetUserId()
        {
            var user = httpContextAccessor.HttpContext.User;
            var userId = userManager.GetUserId(user);
            return userId;
        }
    }
}
