using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DirectBot.DAL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;
    public DbSet<Instagram> Instagrams { get; set; } = null!;
    public DbSet<Proxy> Proxies { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Subscribe> Subscribes { get; set; } = null!;
    public DbSet<Work> Works { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Long relationship from the user
        modelBuilder.Entity<User>()
            .HasMany(c => c.Instagrams).WithOne(c => c.User).HasForeignKey(instagram => instagram.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);


        //Short relationship from the user
        modelBuilder.Entity<User>()
            .HasMany(c => c.Payments).WithOne(c => c.User).HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(c => c.Subscribes).WithOne(c => c.User).HasForeignKey(subscribe => subscribe.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //Auxiliary user relationships
        modelBuilder.Entity<User>()
            .HasOne(c => c.CurrentInstagram).WithOne().HasForeignKey<User>(user => user.CurrentInstagramId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<User>()
            .HasOne(c => c.CurrentWork).WithOne().HasForeignKey<User>(user => user.CurrentWorkId)
            .OnDelete(DeleteBehavior.SetNull);


        //Instagram and works
        modelBuilder.Entity<Instagram>().HasMany(c => c.Works).WithMany(c => c.Instagrams);


        modelBuilder.Entity<Instagram>().HasOne(c => c.Proxy).WithMany(inst => inst.Instagrams)
            .HasForeignKey(c => c.ProxyId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Work>().Property(work => work.InstagramPks).HasConversion(s => string.Join(' ', s),
            s => Array.ConvertAll(s.Split(' ', StringSplitOptions.RemoveEmptyEntries), long.Parse)
                .ToList(), new ValueComparer<List<long>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
        modelBuilder.Entity<Work>().Property(work => work.IntervalPerDivision).HasConversion(s => s.Ticks,
            s => TimeSpan.FromTicks(s));
    }
}