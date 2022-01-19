using DirectBot.Core.Models;
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
    public DbSet<Models.Instagram> Instagrams { get; set; } = null!;
    public DbSet<Proxy> Proxies { get; set; } = null!;
    public DbSet<Subscribe> Subscribes { get; set; } = null!;
    public DbSet<Work> Works { get; set; } = null!;
    // public IQueryable<Proxy> OrderByRandom() => FromExpression(() => OrderByRandom());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(c => c.Instagrams).WithOne(c => c.User);
        modelBuilder.Entity<User>()
            .HasMany(c => c.Subscribes).WithOne(c => c.User);
        modelBuilder.Entity<User>().HasOne(c => c.CurrentInstagram);
        modelBuilder.Entity<User>().HasMany(c => c.CurrentWorks);
        modelBuilder.Entity<Models.Instagram>().HasMany(c => c.Works).WithOne(c => c.Instagram);
        modelBuilder.Entity<Instagram>().HasOne(c => c.Proxy);
    }
}