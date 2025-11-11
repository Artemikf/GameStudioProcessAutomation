using Microsoft.AspNetCore.Routing;

namespace WebApplication1
{
    public static class CustomRouteExtensions
    {
        public static IEndpointRouteBuilder MapCustomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Додаткові маршрути
            endpoints.MapControllerRoute(
                name: "project",
                pattern: "Project/{action=Index}/{id?}");

            return endpoints;
        }
    }
}