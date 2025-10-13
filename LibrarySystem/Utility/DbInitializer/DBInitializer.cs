using LibrarySystem.DataAccess;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibrarySystem.Utility.DbInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DBInitializer> _logger;
        public DBInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<DBInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        public void Initialize()
        {
            try
            {
                //ده بيخلي التطبيق يحدث قاعدة البيانات تلقائيًا لو في تغييرات في الـ Models (زي جداول جديدة أو تعديل أعمدة).
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                //بيشيّك إذا جدول الـ Roles فاضي، لو فاضي، بيضيف الأدوار الأساسية (SuperAdmin, Admin, Company, Customer).
                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.AdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.CompanyRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.CustomerRole)).GetAwaiter().GetResult();

                    //✅ إنشاء أول مستخدم (Super Admin)
                    //بيضيف مستخدم رئيسي في النظام باسمه SuperAdmin@EraaSoft.com
                    var result = _userManager.CreateAsync(new()
                    {
                        Email = "SuperAdmin@EraaSoft.com",
                        EmailConfirmed = true,
                        UserName = "SuperAdmin",
                        Name = "Super Admin"
                    }, "Admin123@").GetAwaiter().GetResult();
                    //✅ ربط المستخدم بدور SuperAdmin
                    //هنا بيربط الحساب ده بدور SuperAdmin علشان يبقى عنده كل الصلاحيات.
                    var user = _userManager.FindByEmailAsync("SuperAdmin@EraaSoft.com").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, SD.SuperAdminRole).GetAwaiter().GetResult();
                }
            }
            //⚠️ لو حصل أي خطأ
            //يسجّل الخطأ في اللوج ويطبع رسالة توضح إن المشكلة ممكن تكون في الاتصال بقاعدة البيانات.
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError("Check connection. Use DB on local server (.)");
            }
        }

    }
}
