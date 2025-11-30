using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Tasky.SaaS.Entities;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Tasky.SaaS.EntityFrameworkCore;

[ConnectionStringName(TaskyNames.SaaSDb)]
public class SaaSDbContext(DbContextOptions<SaaSDbContext> options)
    : AbpDbContext<SaaSDbContext>(options),
        ITenantManagementDbContext,
        IIdentityDbContext,
        ISaaSDbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    
    public DbSet<IdentityUser> Users { get; set; }
    
    public DbSet<IdentityRole> Roles { get; set; }
    
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    
    public DbSet<IdentitySession> Sessions { get; set; }
    
    public DbSet<Edition> Editions { get; set; }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSaaS();
        builder.ConfigureTenantManagement();
        builder.ConfigureIdentity();
    }
}
