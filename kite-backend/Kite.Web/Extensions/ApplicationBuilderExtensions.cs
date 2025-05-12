using Microsoft.AspNetCore.Identity;

namespace Kite.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task EnsureRolesCreatedAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            string[] roles = { "User", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}