using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Tasky.SaaS.EntityFrameworkCore;

[ConnectionStringName(TaskyNames.SaaSDb)]
public interface ISaaSDbContext : IEfCoreDbContext
{
    DbSet<Edition> Editions { get; }
    
    DbSet<Subscription> Subscriptions { get; }
    
    DbSet<Invoice> Invoices { get; }
}
