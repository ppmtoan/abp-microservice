using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Tasky.SaaS.EntityFrameworkCore.Repositories;

public class SubscriptionRepository : EfCoreRepository<ISaaSDbContext, Subscription, Guid>, ISubscriptionRepository
{
    public SubscriptionRepository(IDbContextProvider<ISaaSDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public async Task<Subscription> FindActiveByTenantIdAsync(Guid tenantId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.CreationTime)
            .FirstOrDefaultAsync();
    }
}
