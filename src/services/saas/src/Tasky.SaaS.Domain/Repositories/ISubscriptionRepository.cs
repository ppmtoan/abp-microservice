using System;
using System.Threading.Tasks;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Volo.Abp.Domain.Repositories;

namespace Tasky.SaaS.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription, Guid>
{
    Task<Subscription> FindActiveByTenantIdAsync(Guid tenantId);
}
