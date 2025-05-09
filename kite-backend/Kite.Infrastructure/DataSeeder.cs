using Kite.Domain.Entities;
using Kite.Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;

namespace Kite.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedData(UserManager<ApplicationUser> userManager)
    {
        await SeedUsers.SeedUserData(userManager);
    }
}