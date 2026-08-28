using ITI_Project.Data;
using ITI_Project.DTO;
using ITI_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Project.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly ApplicationDbContext context;

        public CartController(ICartRepository cartRepository, ApplicationDbContext context)
        {
            this.cartRepository = cartRepository;
            this.context = context;
        }

        public async Task<IActionResult> AddItem(int productId, int Qty = 1, int redirect = 0)
        {
            var cartCount = await cartRepository.AddItem(productId, Qty);

            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await cartRepository.RemoveItem(productId);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await cartRepository.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetCartItemCount(string UserId)
        {
            var CartItemCount = await cartRepository.GetCartItemCount(UserId);
            return Ok(CartItemCount);
        }

        public IActionResult DoCheckout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoCheckout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isCheckOut = await cartRepository.DoCheckout(model);
            if (!isCheckOut)
                return RedirectToAction(nameof(OrderFailure));

            return RedirectToAction(nameof(OrderSuccess));
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
