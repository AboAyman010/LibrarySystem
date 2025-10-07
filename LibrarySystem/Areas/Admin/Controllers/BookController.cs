using LibrarySystem.DataAccess;
using LibrarySystem.Models;
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
         var Books=   _context.Books;
       
            return View(Books.ToList());
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
         var book=   _context.Books.FirstOrDefault(e => e.Id == id);
            if(book is null)
            {
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);
            }
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
