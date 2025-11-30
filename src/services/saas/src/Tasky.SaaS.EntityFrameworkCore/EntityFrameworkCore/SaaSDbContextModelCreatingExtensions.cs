using Microsoft.EntityFrameworkCore;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Tasky.SaaS.ValueObjects;
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
            
            // Configure Money Value Objects
            b.OwnsOne(e => e.MonthlyPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("MonthlyPrice").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("MonthlyCurrency").HasMaxLength(3).HasDefaultValue("USD");
            });
            
            b.OwnsOne(e => e.YearlyPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("YearlyPrice").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("YearlyCurrency").HasMaxLength(3).HasDefaultValue("USD");
            });
            
            // Configure FeatureLimits Value Object
            b.OwnsOne(e => e.FeatureLimits, limits =>
            {
                limits.Property(l => l.MaxUsers).HasColumnName("MaxUsers");
                limits.Property(l => l.MaxProjects).HasColumnName("MaxProjects");
                limits.Property(l => l.StorageQuotaGB).HasColumnName("StorageQuotaGB");
                limits.Property(l => l.APICallsPerMonth).HasColumnName("APICallsPerMonth");
                limits.Property(l => l.EnableAdvancedReports).HasColumnName("EnableAdvancedReports");
                limits.Property(l => l.EnablePrioritySupport).HasColumnName("EnablePrioritySupport");
                limits.Property(l => l.EnableCustomBranding).HasColumnName("EnableCustomBranding");
            });
            
            b.HasIndex(e => e.Name);
            b.HasIndex(e => e.IsActive);
        });

        builder.Entity<Subscription>(b =>
        {
            b.ToTable(SaaSDbProperties.DbTablePrefix + "Subscriptions", SaaSDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            // Configure Money Value Object
            b.OwnsOne(s => s.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("USD");
            });
            
            // Configure DateRange Value Object
            b.OwnsOne(s => s.SubscriptionPeriod, period =>
            {
                period.Property(p => p.StartDate).HasColumnName("StartDate");
                period.Property(p => p.EndDate).HasColumnName("EndDate");
            });
            
            b.HasOne(s => s.Edition)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(s => s.EditionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(s => s.TenantId);
            b.HasIndex(s => s.EditionId);
            b.HasIndex(s => s.Status);
        });

        builder.Entity<Invoice>(b =>
        {
            b.ToTable(SaaSDbProperties.DbTablePrefix + "Invoices", SaaSDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            // Configure InvoiceNumber Value Object
            b.OwnsOne(i => i.InvoiceNumber, number =>
            {
                number.Property(n => n.Value).HasColumnName("InvoiceNumber").IsRequired().HasMaxLength(64);
            });
            
            // Configure Money Value Object
            b.OwnsOne(i => i.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("USD");
            });
            
            // Configure DateRange Value Object for Billing Period
            b.OwnsOne(i => i.BillingPeriodRange, period =>
            {
                period.Property(p => p.StartDate).HasColumnName("PeriodStart");
                period.Property(p => p.EndDate).HasColumnName("PeriodEnd");
            });
            
            b.Property(i => i.PaymentMethod).HasMaxLength(128);
            b.Property(i => i.PaymentReference).HasMaxLength(256);
            b.Property(i => i.Notes).HasMaxLength(1024);
            
            b.HasOne(i => i.Subscription)
                .WithMany()
                .HasForeignKey(i => i.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(i => i.TenantId);
            b.HasIndex(i => i.SubscriptionId);
            b.HasIndex(i => i.Status);
            b.HasIndex(i => i.DueDate);
        });
    }
}
