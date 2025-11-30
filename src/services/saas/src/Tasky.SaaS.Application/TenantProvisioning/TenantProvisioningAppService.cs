using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tasky.SaaS.Entities;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Permissions;
using Tasky.SaaS.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;

namespace Tasky.SaaS.TenantProvisioning;

public class TenantProvisioningAppService : ApplicationService, ITenantProvisioningAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantManager _tenantManager;
    private readonly IEditionRepository _editionRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _userManager;

    public TenantProvisioningAppService(
        ITenantRepository tenantRepository,
        ITenantManager tenantManager,
        IEditionRepository editionRepository,
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        IIdentityUserRepository userRepository,
        IdentityUserManager userManager)
    {
        _tenantRepository = tenantRepository;
        _tenantManager = tenantManager;
        _editionRepository = editionRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [UnitOfWork]
    public virtual async Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input)
    {
        // This could be made public or protected with rate limiting
        // For now, require permission
        await CheckPolicyAsync(SaaSPermissions.TenantProvisioning.Default);

        try
        {
            // 1. Validate Edition
            var edition = await _editionRepository.GetAsync(input.EditionId);
            if (edition == null || !edition.IsActive)
            {
                throw new BusinessException(SaaSErrorCodes.EditionNotFound)
                    .WithData("EditionId", input.EditionId);
            }

            // 2. Check if tenant already exists
            var existingTenant = await _tenantRepository.FindByNameAsync(input.TenantName);

            if (existingTenant != null)
            {
                throw new BusinessException(SaaSErrorCodes.TenantAlreadyExists)
                    .WithData("TenantName", input.TenantName);
            }

            // 3. Create Tenant
            var tenant = await _tenantManager.CreateAsync(input.TenantName);
            await _tenantRepository.InsertAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync();

            Guid adminUserId;

            // 4. Create Admin User in Tenant context
            using (CurrentTenant.Change(tenant.Id))
            {
                var adminUser = new IdentityUser(
                    GuidGenerator.Create(),
                    input.AdminUserName ?? input.AdminEmail,
                    input.AdminEmail,
                    tenant.Id
                );

                await _userManager.CreateAsync(adminUser, input.AdminPassword);
                
                // Assign admin role (assuming it exists)
                await _userManager.AddToRoleAsync(adminUser, "admin");

                await CurrentUnitOfWork.SaveChangesAsync();
                adminUserId = adminUser.Id;
            }

            // 5. Create Subscription
            var price = input.BillingPeriod == BillingPeriod.Monthly 
                ? edition.MonthlyPrice 
                : edition.YearlyPrice;

            var subscription = new Subscription(
                GuidGenerator.Create(),
                tenant.Id,
                input.EditionId,
                input.BillingPeriod,
                Clock.Now,
                price,
                autoRenew: true,
                trialDays: input.TrialDays
            );

            await _subscriptionRepository.InsertAsync(subscription);
            await CurrentUnitOfWork.SaveChangesAsync();

            // 6. Create Initial Invoice (if not trial)
            if (!input.TrialDays.HasValue || input.TrialDays.Value == 0)
            {
                var invoice = new Invoice(
                    GuidGenerator.Create(),
                    tenant.Id,
                    subscription.Id,
                    GenerateInvoiceNumber(tenant.Id),
                    Clock.Now,
                    Clock.Now.AddDays(30), // Due in 30 days
                    price,
                    input.BillingPeriod,
                    subscription.StartDate,
                    subscription.EndDate
                );

                await _invoiceRepository.InsertAsync(invoice);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            Logger.LogInformation($"Successfully provisioned tenant: {input.TenantName} with admin: {input.AdminEmail}");

            return new TenantProvisioningResultDto
            {
                TenantId = tenant.Id,
                TenantName = tenant.Name,
                SubscriptionId = subscription.Id,
                AdminUserId = adminUserId,
                AdminEmail = input.AdminEmail,
                Success = true,
                Message = "Tenant provisioned successfully"
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to provision tenant: {input.TenantName}");
            throw;
        }
    }

    private string GenerateInvoiceNumber(Guid tenantId)
    {
        return $"INV-{tenantId.ToString().Substring(0, 8).ToUpper()}-{Clock.Now:yyyyMMddHHmmss}";
    }
}
