using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication1.Services;

namespace WebApplication1.Filters
{
    public class AdminAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IAuthService _authService;

        public AdminAuthorizationFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_authService.IsAuthenticated())
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (!_authService.IsAdmin())
            {
                context.Result = new ViewResult { ViewName = "AccessDenied" };
                return;
            }
        }
    }

    public class AuthAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IAuthService _authService;

        public AuthAuthorizationFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_authService.IsAuthenticated())
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}