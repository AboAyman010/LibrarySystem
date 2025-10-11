using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models.ViewModel;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystem.Areas.Customer.Controllers;

[Area(SD.CustomerArea)]
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
   private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(BookFilterVM bookFilterVM, int page = 1)
    {
        var Books = _context.Books.Include(e => e.Loans).AsQueryable();
        if(bookFilterVM.BookTitle is not null)
        {
            Books = Books.Where(e => e.Title.Contains(bookFilterVM.BookTitle));
            ViewBag.BookTitle = bookFilterVM.BookTitle;
        }
        if (bookFilterVM.BookAuthor is not null)
        {
            Books = Books.Where(e => e.Author.Contains(bookFilterVM.BookAuthor));
            ViewBag.BookAuthor = bookFilterVM.BookAuthor;
        }
        ViewBag.Books = _context.Books
         .Select(b => new { b.Author })
         .Distinct()
         .ToList();



        //pagination
        double totalPages = Math.Ceiling(Books.Count() / 3.0);
        int CurrentPage = page;
        ViewBag.totalPages = totalPages;
        ViewBag.CurrentPage = CurrentPage;
        Books = Books.Skip((page - 1) * 3).Take(3);

        return View(Books.ToList());
    }
    public IActionResult Details(int id)
    {
       var Books= _context.Books.Include(e => e.Loans).ThenInclude(e => e.Reader).FirstOrDefault(b => b.Id == id);
        if (Books == null)
        {
            return NotFound();
        }
        return View(Books);
    }
   
    public IActionResult Privacy()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
