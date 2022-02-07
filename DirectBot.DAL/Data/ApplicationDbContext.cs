using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Instagram = DirectBot.DAL.Models.Instagram;
using Proxy = DirectBot.DAL.Models.Proxy;
using Subscribe = DirectBot.DAL.Models.Subscribe;
using User = DirectBot.DAL.Models.User;
using Work = DirectBot.DAL.Models.Work;

namespace DirectBot.DAL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Instagram> Instagrams { get; set; } = null!;
    public DbSet<Proxy> Proxies { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Subscribe> Subscribes { get; set; } = null!;
    public DbSet<Work> Works { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(c => c.Instagrams).WithOne(c => c.User).HasForeignKey(instagram => instagram.UserId);
        modelBuilder.Entity<User>()
            .HasOne(c => c.CurrentInstagram).WithOne().HasForeignKey<User>(user => user.CurrentInstagramId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<User>()
            .HasOne(c => c.CurrentWork).WithOne().HasForeignKey<User>(user => user.CurrentWorkId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasMany(c => c.Payments).WithOne(c => c.User).HasForeignKey(payment => payment.UserId);

        modelBuilder.Entity<User>()
            .HasMany(c => c.Subscribes).WithOne(c => c.User).HasForeignKey(subscribe => subscribe.UserId);
        modelBuilder.Entity<User>()
            .HasMany(c => c.Works).WithOne(c => c.User).HasForeignKey(work => work.UserId);

        modelBuilder.Entity<Instagram>().HasMany(c => c.Works).WithMany(c => c.Instagrams);

        modelBuilder.Entity<Instagram>().HasOne(c => c.Proxy).WithMany(inst => inst.Instagrams)
            .HasForeignKey(c => c.ProxyId);
    }
}