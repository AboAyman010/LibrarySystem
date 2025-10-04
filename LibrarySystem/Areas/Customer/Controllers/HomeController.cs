using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
   private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var Books = _context.Books.Include(e => e.Loans);
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
