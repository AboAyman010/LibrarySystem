using LibrarySystem.Models;
using LibrarySystem.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Areas.Customer.Controllers
{
    [Area(SD.CustomerArea)]
    [Authorize]
    public class CartController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> AddToCart(CartRequestVM cartRequestVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == cartRequestVM.BookId);

            if (cart is not null)
            {
                cart.Count += cartRequestVM.Count;
            }
            else
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    BookId = cartRequestVM.BookId,
                    Count = cartRequestVM.Count
                });
            }


            await _cartRepository.CommitAsync();

            TempData["success-notification"] = "Add Product To Cart Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public async Task<IActionResult> Index(string? code = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.book]);

            //var totalPrice = carts.Sum(e => e.book.Price * e.Count);

            //if (code is not null)
            //{
            //    var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code);
            //    if (promotion is null || !promotion.Status || DateTime.UtcNow > promotion.ValidTo)
            //    {
            //        TempData["error-notification"] = "Invalid Code OR Expired";
            //    }
            //    else
            //    {
            //        promotion.TotalUsed += 1;
            //        await _promotionRepository.CommitAsync();

            //        //totalPrice = totalPrice - (totalPrice * 0.05m);
            //        TempData["success-notification"] = "Apply Promotion";
            //    }
            //}

            //ViewBag.TotalPrice = totalPrice;

            return View(carts);
        }
        public async Task<IActionResult> IncrementCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);

            if (cart is null)
                return NotFound();

            cart.Count += 1;
            await _cartRepository.CommitAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecrementCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);

            if (cart is null)
                return NotFound();

            if (cart.Count > 1)
            {
                cart.Count -= 1;
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);

            if (cart is null)
                return NotFound();

            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();

            return RedirectToAction("Index");
        }
      
    }
}
