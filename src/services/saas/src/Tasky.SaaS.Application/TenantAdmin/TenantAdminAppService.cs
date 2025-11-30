using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;

namespace Tasky.SaaS.TenantAdmin;

public class TenantAdminAppService : ApplicationService, ITenantAdminAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IEditionRepository _editionRepository;

    public TenantAdminAppService(
        ITenantRepository tenantRepository,
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        IIdentityUserRepository userRepository,
        IEditionRepository editionRepository)
    {
        _tenantRepository = tenantRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _userRepository = userRepository;
        _editionRepository = editionRepository;
    }

    public async Task<TenantDashboardDto> GetDashboardAsync()
    {
        if (!CurrentTenant.IsAvailable)
        {
            throw new BusinessException(SaaSErrorCodes.TenantNotAvailable);
        }

        var tenantId = CurrentTenant.Id.Value;
        var tenant = await _tenantRepository.GetAsync(tenantId);

        var subscriptionsQuery = await _subscriptionRepository.GetQueryableAsync();
        var subscription = await AsyncExecuter.FirstOrDefaultAsync(
            subscriptionsQuery
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreationTime)
        );

        if (subscription == null)
        {
            throw new BusinessException(SaaSErrorCodes.SubscriptionNotFound);
        }
        
        // Load the edition separately
        var edition = await _editionRepository.GetAsync(subscription.EditionId);

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var invoices = await AsyncExecuter.ToListAsync(
            invoicesQuery.Where(i => i.TenantId == tenantId)
        );

        var userCount = (int)await _userRepository.GetCountAsync();

        var featureLimits = string.IsNullOrEmpty(edition?.FeatureLimits)
            ? new System.Collections.Generic.Dictionary<string, object>()
            : JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(edition.FeatureLimits);

        return new TenantDashboardDto
        {
            TenantId = tenantId,
            TenantName = tenant.Name,
            EditionName = edition?.DisplayName,
            FeatureLimits = featureLimits,
            SubscriptionStatus = subscription.Status.ToString(),
            SubscriptionEndDate = subscription.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            DaysRemaining = (subscription.EndDate - Clock.Now).Days,
            CurrentPlanPrice = subscription.Price,
            BillingPeriod = subscription.BillingPeriod.ToString(),
            TotalUsers = userCount,
            PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Pending),
            OverdueInvoices = invoices.Count(i => i.Status == InvoiceStatus.Overdue)
        };
    }
}
