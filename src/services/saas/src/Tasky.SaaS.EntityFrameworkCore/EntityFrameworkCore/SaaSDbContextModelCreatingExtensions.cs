using Microsoft.EntityFrameworkCore;
using Tasky.SaaS.Entities;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Tasky.SaaS.EntityFrameworkCore;

public static class SaaSDbContextModelCreatingExtensions
{
    public static void ConfigureSaaS(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Edition>(b =>
        {
            b.ToTable(SaaSDbProperties.DbTablePrefix + "Editions", SaaSDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            b.Property(e => e.Name).IsRequired().HasMaxLength(128);
            b.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
            b.Property(e => e.Description).HasMaxLength(1024);
            b.Property(e => e.MonthlyPrice).HasColumnType("decimal(18,2)");
            b.Property(e => e.YearlyPrice).HasColumnType("decimal(18,2)");
            b.Property(e => e.FeatureLimits).HasColumnType("nvarchar(max)");
            
            b.HasIndex(e => e.Name);
            b.HasIndex(e => e.IsActive);
        });

        builder.Entity<Subscription>(b =>
        {
            b.ToTable(SaaSDbProperties.DbTablePrefix + "Subscriptions", SaaSDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            b.Property(s => s.Price).HasColumnType("decimal(18,2)");
            
            b.HasOne(s => s.Edition)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(s => s.EditionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(s => s.TenantId);
            b.HasIndex(s => s.EditionId);
            b.HasIndex(s => s.Status);
            b.HasIndex(s => s.EndDate);
        });

        builder.Entity<Invoice>(b =>
        {
            b.ToTable(SaaSDbProperties.DbTablePrefix + "Invoices", SaaSDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            b.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(64);
            b.Property(i => i.Amount).HasColumnType("decimal(18,2)");
            b.Property(i => i.PaymentMethod).HasMaxLength(128);
            b.Property(i => i.PaymentReference).HasMaxLength(256);
            b.Property(i => i.Notes).HasMaxLength(1024);
            
            b.HasOne(i => i.Subscription)
                .WithMany()
                .HasForeignKey(i => i.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(i => i.TenantId);
            b.HasIndex(i => i.SubscriptionId);
            b.HasIndex(i => i.InvoiceNumber).IsUnique();
            b.HasIndex(i => i.Status);
            b.HasIndex(i => i.DueDate);
        });
    }
}
