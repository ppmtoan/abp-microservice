using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Tasky.SaaS.EntityFrameworkCore;

[ConnectionStringName(TaskyNames.SaaSDb)]
public class SaaSDbContext(DbContextOptions<SaaSDbContext> options)
    : AbpDbContext<SaaSDbContext>(options),
        ITenantManagementDbContext,
        ISaaSDbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    
    public DbSet<Edition> Editions { get; set; }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSaaS();
        builder.ConfigureTenantManagement();
    }
}
