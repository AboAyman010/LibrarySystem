using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    public class HomeController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = $"{SD.SuperAdminRole},{SD.AdminArea}")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
