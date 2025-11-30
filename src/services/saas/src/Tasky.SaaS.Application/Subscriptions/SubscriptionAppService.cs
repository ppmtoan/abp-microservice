using System;
using System.Linq;
using System.Threading.Tasks;
using Tasky.SaaS.Entities;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Permissions;
using Tasky.SaaS.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Tasky.SaaS.Subscriptions;

public class SubscriptionAppService : CrudAppService<Subscription, SubscriptionDto, Guid, PagedAndSortedResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>, ISubscriptionAppService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IEditionRepository _editionRepository;

    public SubscriptionAppService(
        ISubscriptionRepository repository,
        IEditionRepository editionRepository) : base(repository)
    {
        _subscriptionRepository = repository;
        _editionRepository = editionRepository;
        
        GetPolicyName = SaaSPermissions.Subscriptions.Default;
        GetListPolicyName = SaaSPermissions.Subscriptions.Default;
        CreatePolicyName = SaaSPermissions.Subscriptions.Create;
        UpdatePolicyName = SaaSPermissions.Subscriptions.Update;
        DeletePolicyName = SaaSPermissions.Subscriptions.Delete;
    }

    public override async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto input)
    {
        var edition = await _editionRepository.GetAsync(input.EditionId);
        
        if (edition == null)
        {
            throw new BusinessException(SaaSErrorCodes.EditionNotFound)
                .WithData("EditionId", input.EditionId);
        }

        var price = input.Price > 0 ? input.Price : 
            (input.BillingPeriod == BillingPeriod.Monthly ? edition.MonthlyPrice : edition.YearlyPrice);

        var subscription = new Subscription(
            GuidGenerator.Create(),
            input.TenantId ?? CurrentTenant.Id,
            input.EditionId,
            input.BillingPeriod,
            input.StartDate ?? Clock.Now,
            price,
            input.AutoRenew,
            input.TrialDays
        );

        await _subscriptionRepository.InsertAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await MapToGetOutputDtoAsync(subscription);
    }

    public override async Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto input)
    {
        await CheckUpdatePolicyAsync();

        var subscription = await GetEntityByIdAsync(id);
        
        subscription.UpdateBillingPeriod(input.BillingPeriod, input.Price);
        subscription.AutoRenew = input.AutoRenew;

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await MapToGetOutputDtoAsync(subscription);
    }

    public async Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync()
    {
        if (!CurrentTenant.IsAvailable)
        {
            throw new BusinessException(SaaSErrorCodes.TenantNotAvailable);
        }

        var query = await _subscriptionRepository.GetQueryableAsync();
        var subscription = await AsyncExecuter.FirstOrDefaultAsync(
            query.Where(s => s.TenantId == CurrentTenant.Id)
                .OrderByDescending(s => s.CreationTime)
        );

        if (subscription == null)
        {
            throw new BusinessException(SaaSErrorCodes.SubscriptionNotFound);
        }

        return await MapToGetOutputDtoAsync(subscription);
    }

    public async Task RenewAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Renew();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Cancel();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task SuspendAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Suspend();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Activate();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    protected override async Task<IQueryable<Subscription>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
    {
        return await base.CreateFilteredQueryAsync(input);
    }

    protected override IQueryable<Subscription> ApplyDefaultSorting(IQueryable<Subscription> query)
    {
        return query.OrderByDescending(s => s.CreationTime);
    }
}
