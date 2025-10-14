using LibrarySystem.Models;
using LibrarySystem.Models.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var updateUser = user.Adapt<UpdatePersonalInfoVM>();
            updateUser.ExistingImage = user.ProfilePicture;
            return View(updateUser);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UpdatePersonalInfoVM updatePersonalInfoVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", updatePersonalInfoVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();


            user.Name = updatePersonalInfoVM.Name;
            user.Email = updatePersonalInfoVM.Email;
            user.PhoneNumber = updatePersonalInfoVM.PhoneNumber;
            user.Street = updatePersonalInfoVM.Street;
            user.State = updatePersonalInfoVM.State;
            user.City = updatePersonalInfoVM.City;
            user.ZipCode = updatePersonalInfoVM.ZipCode;


            if (updatePersonalInfoVM.ProfileImage != null && updatePersonalInfoVM.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // يتأكد إن الفولدر موجود
                Directory.CreateDirectory(uploadsFolder);

                // اسم جديد للصورة
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updatePersonalInfoVM.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // حفظ الصورة
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updatePersonalInfoVM.ProfileImage.CopyToAsync(fileStream);
                }

                // حذف الصورة القديمة لو موجودة
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldPath = Path.Combine(uploadsFolder, user.ProfilePicture);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // تخزين اسم الصورة الجديدة
                user.ProfilePicture = uniqueFileName;
            }

            //  تحديث المستخدم
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Index", updatePersonalInfoVM);
            }

            //  تغيير الباسورد لو المستخدم دخّل القديم والجديد
            if (!string.IsNullOrWhiteSpace(updatePersonalInfoVM.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(updatePersonalInfoVM.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(
                    user,
                    updatePersonalInfoVM.CurrentPassword,
                    updatePersonalInfoVM.NewPassword
                );

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Index", updatePersonalInfoVM);
                }
            }

            //  تمرير الصورة الحالية عشان تعرض في الـ View
            updatePersonalInfoVM.ExistingImage = user.ProfilePicture;

            return RedirectToAction(nameof(Index), "Profile", new { area = "Identity" });
        }
    }
}
