using LibrarySystem.DataAccess;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.AdminArea}")]
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

        [HttpPost]
        public IActionResult Create(Book book,IFormFile ImageUrl)
        {
         
            if (ImageUrl is not null && ImageUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images",
                    fileName);

                //save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageUrl.CopyTo(stream);
                }

                //save img name in db
                book.ImageUrl = fileName;

                //Save In Db
                _context.Books.Add(book);
                _context.SaveChanges();

                TempData["success-notification"] = "Add Book Successfully";
                Response.Cookies.Append("success-notification", "Add Book Successfully", new()
                {
                    Secure = true,
                    Expires = DateTime.Now.AddDays(1)
                });
                return RedirectToAction(nameof(Index));
              
            }
            return BadRequest();
               
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
        public IActionResult Edit(Book book, IFormFile? ImageUrl)
        {
            //عشان لو مرفعتش صورة ميضربش ايرور يروح يجيب الصورة اللي مرفوعة قبل كدا من الداتابيز
            var BookInDb = _context.Books.AsNoTracking().FirstOrDefault(e => e.Id == book.Id);
            if (BookInDb is null)
            {
                return BadRequest();
            }


            if (ImageUrl is not null && ImageUrl.Length > 0)
            {
                // إنشاء اسم فريد للصورة الجديدة
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images",
                    fileName);

                //save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageUrl.CopyTo(stream);
                }

                //delete old img from wwwroot
                // حذف الصورة القديمة (إن وجدت)

                if (!string.IsNullOrEmpty(BookInDb.ImageUrl))
                {
                    var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images",
                   BookInDb.ImageUrl);
                    if (System.IO.File.Exists(OldFilePath))
                    {
                        System.IO.File.Delete(OldFilePath);
                    }

                }
                //update img name in db
                book.ImageUrl = fileName;
            }
            else
            {
                // لو المستخدم ما رفعش صورة، نحافظ على القديمة

                book.ImageUrl = BookInDb.ImageUrl;
            }
        //update in db
            _context.Books.Update(book);
            _context.SaveChanges();
            TempData["success-notification"] = "Update Book Successfully";
            return RedirectToAction(nameof(Index));
        }
       

        public IActionResult Delete(int id)
        {
            var book = _context.Books.FirstOrDefault(e => e.Id == id);
            if (book is null)
            {
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);
            }
            //delete old img from wwwroot

            var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images",
               book.ImageUrl);
            if (System.IO.File.Exists(OldFilePath))
            {
                System.IO.File.Delete(OldFilePath);
            }


            //Remove in db
            _context.Books.Remove(book);
            _context.SaveChanges();
            TempData["success-notification"] = "Delete Book Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
