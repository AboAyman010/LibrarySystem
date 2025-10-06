using LibrarySystem.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var Books = _context.Books.Include(e=>e.Loans).OrderBy(e=>e.TotalCopies).OrderBy(e=>e.AvailableCopies);
            return View(Books.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
    }
}
