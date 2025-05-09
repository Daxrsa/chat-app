using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Seed;

public static class SeedUsers
{
    private const string SeedUsersPassword = "P@ssword123";
    public static async Task SeedUserData(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userDaorsa = new ApplicationUser
        {
            UserName = "daxrsa",
            Email = "daorsahyseni@gmail.com",
            FirstName = "Daorsa",
            LastName = "Hyseni",
        };

        var userMaverick = new ApplicationUser
        {
            UserName = "mav",
            Email = "asd@qwe.com",
            FirstName = "Tester",
            LastName = "Maverick",
        };

        var userJohn = new ApplicationUser
        {
            UserName = "john",
            Email = "john@example.com",
            FirstName = "john",
            LastName = "doe"
        };

        var userBob = new ApplicationUser
        {
            UserName = "bob",
            Email = "bob@example.com",
            FirstName = "bob",
            LastName = "doe",
        };

        var userJane = new ApplicationUser
        {
            UserName = "jane",
            Email = "jane@example.com",
            FirstName = "jane",
            LastName = "doe"
        };

        await SeedUser(userManager, userDaorsa, SeedUsersPassword);
        await SeedUser(userManager, userMaverick, SeedUsersPassword);
        await SeedUser(userManager, userJohn, SeedUsersPassword);
        await SeedUser(userManager, userBob, SeedUsersPassword);
        await SeedUser(userManager, userJane, SeedUsersPassword);
    }

    private static async Task SeedUser(UserManager<ApplicationUser> userManager,
        ApplicationUser user, string password)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email);
        if (existingUser == null)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed user {user.Email}: {errors}");
            }
        }
    }
}