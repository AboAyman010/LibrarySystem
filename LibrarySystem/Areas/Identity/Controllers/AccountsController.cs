using LibrarySystem.Models;
using LibrarySystem.Models.ViewModel;
using LibrarySystem.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace LibrarySystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<UserOTP> _userOTP;


        public AccountsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepository<UserOTP> userOTP)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userOTP = userOTP;

        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {

            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
          
            if (!ModelState.IsValid)
            {
               
                return View(registerVM);
            }

            ApplicationUser applicationUser = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                City = registerVM.City,
                Street = registerVM.Street,
                State = registerVM.State,
                ZipCode = registerVM.ZipCode,
                UserName = registerVM.UserName,
            };
            //ApplicationUser applicationUser = registerVM.Adapt<ApplicationUser>();
            var Result = await _userManager.CreateAsync(applicationUser,registerVM.Password);
            if (!Result.Succeeded)
            {
                foreach (var item in Result.Errors)
                {
                    Console.WriteLine($"Error: {item.Description}");

                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(registerVM);

            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var Link = Url.Action("ConfirmEmail", "Accounts", new
            {
                area = "Identity"
            }
               , Request.Scheme
            );
            await _emailSender.SendEmailAsync(applicationUser.Email, "Confirm Your Email", $"<h1>Confirm Your Email By Click Here<a href='{Link}'>Here</a></h1>");


            TempData["success-notification"] = "Create User Successfully, Confirm Your Email!";

            return RedirectToAction("Login", "Accounts", new { area = "Identity" });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailOrUserName);
            if (user is null)
            {
                TempData["error-notification"] = "Invalid User Name Or  Password";
                return View(loginVM);
            }
            var result = await  _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)

                    TempData["error-notification"] = "Too Many Attempts";

                TempData["error-notification"] = "Invalid User Name Or  Password";


                return View(loginVM);
            }
            if (!user.EmailConfirmed)
            {
                TempData["error-notification"] = "Confirm Your Email First";


                return View(loginVM);
            }

            //لو اليوسر معموله بلوك ميقدرش يعمل لوجن
            if (!user.LockoutEnabled)
            {
                TempData["error-notification"] = $"You Have Block till{user.LockoutEnd} ";


                return View(loginVM);
            }
            TempData["success-notification"] = "Login Successfully";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM )
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);
            if (user is null)
            {
                TempData["error-notification"] = "Invalid User Name Or  Password";
                return View(resendEmailConfirmationVM);
            }
            if (user.EmailConfirmed)
            {
                TempData["error-notification"] = "Already Confirmed!";
                return View(resendEmailConfirmationVM);

            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", token = token, UserId = user.Id }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email", $"<h1>Confirm Your Email By Click Here<a href='{Link}'>Here</a></h1>");

            TempData["success-notification"] = "Send Email Successfully, Confirm Your Email!";

            return RedirectToAction("Login", "Accounts", new { area = "Identity" });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            return View();

        }
        [HttpGet]
        public IActionResult ResetPassword(string UserId)
        {
            return View(new ResetPasswordVM()
            {
                ApplicationUserId = UserId
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);

            if (user is null)
            {
                TempData["error-notification"] = "Invalid User name Or Email";
                return View(resetPasswordVM);
            }

            var userOTP = (await _userOTP.GetAsync(e => e.ApplicationUserId == resetPasswordVM.ApplicationUserId)).OrderBy(e => e.Id).LastOrDefault();

            if (userOTP is null)
                return NotFound();

            if (userOTP.OTPNumber != resetPasswordVM.OTPNumber)
            {
                TempData["error-notification"] = "Invalid OTP";
                return View(resetPasswordVM);
            }

            if (DateTime.UtcNow > userOTP.ValidTo)
            {
                TempData["error-notification"] = "Expired OTP";
                return View(resetPasswordVM);
            }

            TempData["success-notification"] = "Success OTP";
            return RedirectToAction("NewPassword", "Accounts", new { area = "Identity", UserId = user.Id });
        }
        [HttpGet]
        public IActionResult NewPassword(string UserId)
        {
            return View(new NewPasswordVM()
            {
                ApplicationUserId = UserId
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(newPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);

            if (user is null)
            {
                TempData["error-notification"] = "Invalid User name Or Email";
                return View(newPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            TempData["success-notification"] = "Change Password Successfully!";
            return RedirectToAction("Login", "Accounts", new { area = "Identity" });
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts", new { area = "Identity" });
        }
    }
        
    }

