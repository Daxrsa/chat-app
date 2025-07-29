using System.Reflection;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kite.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<ApplicationFile> Files { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

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

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamptz");
            entity.HasMany(u => u.Reactions)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("timestamptz");
            entity.Property(e => e.ResendRequestTime).HasColumnType("timestamptz");
            entity.Property(e => e.Status).HasConversion<int>();
            entity.HasOne(e => e.UserOne)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UserTwo)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(fr => fr.Friendship)
                .WithOne(f => f.FriendRequest)
                .HasForeignKey<Friendship>(f => f.FriendRequestId)
                .IsRequired(false);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedOnAdd();
            entity.Property(e => e.FriendRequestId).HasColumnType("uuid");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamptz");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(n => n.Receiver)
                .WithMany()
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.HasOne(n => n.Sender)
                .WithMany()
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Property(n => n.Message)
                .IsRequired();
            entity.Property(n => n.Type)
                .IsRequired();
            entity.Property(n => n.IsRead)
                .HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamptz");
            entity.Property(e => e.ReadAt).HasColumnType("timestamptz");
        });
        
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamptz");
            entity.HasMany(p => p.Reactions)
                .WithOne()
                .HasForeignKey(r => r.EntityId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<ApplicationFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.Post)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            entity.Property(e => e.UploadedAt).HasColumnType("timestamptz");
        });
        
        modelBuilder.Entity<Reaction>()
            .HasIndex(r => new { r.UserId, r.EntityId, r.EntityType })
            .IsUnique();
        
        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reactions)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Reaction>()
            .Property(r => r.EntityType)
            .HasConversion<int>();

        modelBuilder.Entity<Reaction>()
            .Property(r => r.ReactionType)
            .HasConversion<int>();
    }
}