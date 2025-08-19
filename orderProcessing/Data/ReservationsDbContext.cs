using Microsoft.EntityFrameworkCore;
using Data.Entities;
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<PhoneReservation> PhoneReservations => Set<PhoneReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<PhoneReservation>();
        e.ToTable("PhoneReservations");
        e.HasKey(x => x.Id);
        //PhoneInfo
        e.Property(x => x.PlanType).HasMaxLength(32);
        e.Property(x => x.Storage).HasMaxLength(16).IsRequired();
        e.Property(x => x.Color).HasMaxLength(32);
        e.Property(x => x.HasTradeIn).IsRequired();
        e.Property(x => x.ExtendedCoverage).IsRequired();
        e.Property(x => x.PaymentOption).HasMaxLength(32).IsRequired();
        e.Property(x => x.AddSmartwatch).IsRequired();
        e.Property(x => x.SmartwatchQty);
        e.Property(x => x.AddBuds).IsRequired();
        e.Property(x => x.BudsQty);
        e.Property(x => x.AddCharger).IsRequired();
        e.Property(x => x.ChargerQty);
        //CustomerInfo
        e.Property(x => x.ClientId).IsRequired();
        e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        e.Property(x => x.Email).HasMaxLength(256).IsRequired();
        e.Property(x => x.Phone).HasMaxLength(64).IsRequired();
        e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        //Indexes
        e.HasIndex(x => x.ClientId);
        e.HasIndex(x => x.CreatedAt);
    }
}