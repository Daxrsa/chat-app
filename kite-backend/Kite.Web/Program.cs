using Kite.Application.Interfaces;
using Kite.Application.Services;
using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Kite.Infrastructure;
using Kite.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// try
// {
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     logger.LogInformation("Starting database migration and data seeding.");
//
//     var context = services.GetRequiredService<AppDbContext>();
//     var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//
//     await context.Database.MigrateAsync();
//     logger.LogInformation("Database migration completed.");
//
//     await DataSeeder.SeedData(userManager);
//     logger.LogInformation("Data seeding completed.");
// }
// catch (Exception ex)
// {
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     logger.LogError(ex, "An error occurred during migration");
// }

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kite API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<ApplicationUser>();

app.Run();