using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineShop.Web.Filters
{
    public class AdminAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var roleId = context.HttpContext.Session.GetInt32("RoleId");

            // Không phải Admin → đá về trang Login của USER (đúng)
            if (roleId != 1)
            {
                context.Result = new RedirectToActionResult(
                    "Login",        // action
                    "Account",      // controller
                    null
                );
                return;
            }

            await next();
        }
    }
}
