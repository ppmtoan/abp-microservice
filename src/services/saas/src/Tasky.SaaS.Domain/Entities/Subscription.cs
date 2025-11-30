using System;
using Tasky.SaaS.Enums;
using Tasky.SaaS.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Tasky.SaaS.Entities;

/// <summary>
/// Represents a tenant's subscription to an edition with billing information
/// </summary>
public class Subscription : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    public Guid EditionId { get; set; }
    
    public Edition Edition { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateRange SubscriptionPeriod { get; set; }
    
    public DateTime? NextBillingDate { get; set; }
    
    public bool AutoRenew { get; set; }
    
    public Money Price { get; set; }
    
    public SubscriptionStatus Status { get; set; }
    
    public int? TrialDays { get; set; }
    
    public DateTime? TrialEndDate { get; set; }

    protected Subscription()
    {
    }

    public Subscription(
        Guid id,
        Guid? tenantId,
        Guid editionId,
        BillingPeriod billingPeriod,
        DateTime startDate,
        Money price,
        bool autoRenew = true,
        int? trialDays = null) : base(id)
    {
        TenantId = tenantId;
        EditionId = editionId;
        BillingPeriod = billingPeriod;
        Price = price;
        AutoRenew = autoRenew;
        TrialDays = trialDays;
        
        if (trialDays.HasValue && trialDays.Value > 0)
        {
            Status = SubscriptionStatus.Trial;
            TrialEndDate = startDate.AddDays(trialDays.Value);
            SubscriptionPeriod = new DateRange(startDate, TrialEndDate.Value);
        }
        else
        {
            Status = SubscriptionStatus.Active;
            var endDate = CalculateEndDate(startDate, billingPeriod);
            SubscriptionPeriod = new DateRange(startDate, endDate);
        }
        
        NextBillingDate = CalculateNextBillingDate();
    }

    public void Renew()
    {
        if (Status == SubscriptionStatus.Trial && TrialEndDate.HasValue)
        {
            // Transition from trial to active
            Status = SubscriptionStatus.Active;
            var endDate = CalculateEndDate(TrialEndDate.Value, BillingPeriod);
            SubscriptionPeriod = new DateRange(TrialEndDate.Value, endDate);
        }
        else
        {
            SubscriptionPeriod = BillingPeriod == BillingPeriod.Yearly 
                ? SubscriptionPeriod.ExtendByYears(1)
                : SubscriptionPeriod.ExtendByMonths(1);
        }
        
        NextBillingDate = CalculateNextBillingDate();
    }

    public void Renew(Money newPrice)
    {
        Renew();
        Price = newPrice;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        AutoRenew = false;
        NextBillingDate = null;
    }

    public void Suspend()
    {
        Status = SubscriptionStatus.Suspended;
    }

    public void Activate()
    {
        Status = SubscriptionStatus.Active;
    }

    public void MarkAsExpired()
    {
        Status = SubscriptionStatus.Expired;
        AutoRenew = false;
    }

    public bool IsActive()
    {
        return Status == SubscriptionStatus.Active && SubscriptionPeriod.IsActive();
    }

    public int DaysRemaining()
    {
        return SubscriptionPeriod.DaysRemaining();
    }

    private DateTime CalculateEndDate(DateTime startDate, BillingPeriod period)
    {
        return period switch
        {
            BillingPeriod.Monthly => startDate.AddMonths(1),
            BillingPeriod.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };
    }

    private DateTime? CalculateNextBillingDate()
    {
        if (!AutoRenew || Status == SubscriptionStatus.Cancelled)
        {
            return null;
        }

        return SubscriptionPeriod.EndDate;
    }

    public void UpdateBillingPeriod(BillingPeriod newPeriod, Money newPrice)
    {
        BillingPeriod = newPeriod;
        Price = newPrice;
        var endDate = CalculateEndDate(SubscriptionPeriod.StartDate, newPeriod);
        SubscriptionPeriod = new DateRange(SubscriptionPeriod.StartDate, endDate);
        NextBillingDate = CalculateNextBillingDate();
    }
}
