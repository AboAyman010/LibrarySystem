using LibrarySystem.Models;
using LibrarySystem.Models.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibrarySystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;


        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ApplicationUser applicationUser = registerVM.Adapt<ApplicationUser>();
            var Result = await _userManager.CreateAsync(applicationUser,registerVM.Password);
            if (!Result.Succeeded)
            {
                foreach (var item in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(registerVM);

            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var Link = Url.Action("ConfirmEmail", "Account", new
            {
                area = "Identity"
            }
               , Request.Scheme
            );
            await _emailSender.SendEmailAsync(applicationUser.Email, "Confirm Your Email", $"<h1>Confirm Your Email By Click Here<a href='{Link}'>Here</a></h1>");


            TempData["success-notification"] = "Create User Successfully, Confirm Your Email!";

            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
    }
}
