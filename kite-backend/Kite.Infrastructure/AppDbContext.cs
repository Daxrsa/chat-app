using System.Reflection;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kite.Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IConfiguration _configuration;
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception(
                "The connection string 'DefaultConnection' is missing or empty in the configuration.");
        }

        optionsBuilder
            .UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("kite");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<ApplicationUser>().Property(u => u.FirstName).HasMaxLength(20);
        modelBuilder.Entity<ApplicationUser>().Property(u => u.LastName).HasMaxLength(20);
        
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(f => new { f.SenderId, f.ReceiverId });
            
            entity.HasOne(f => f.UserOne)
                .WithMany(u => u.FriendshipsInitiated)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(f => f.UserTwo)
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}