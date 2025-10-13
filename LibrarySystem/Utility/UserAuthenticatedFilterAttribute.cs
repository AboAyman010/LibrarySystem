using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Utility
{
    public class UserAuthenticatedFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { area = "Customer" });
            }
        }
    }
}
