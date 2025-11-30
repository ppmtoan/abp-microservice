using System;
using Tasky.SaaS.Enums;
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
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime? NextBillingDate { get; set; }
    
    public bool AutoRenew { get; set; }
    
    public decimal Price { get; set; }
    
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
        decimal price,
        bool autoRenew = true,
        int? trialDays = null) : base(id)
    {
        TenantId = tenantId;
        EditionId = editionId;
        BillingPeriod = billingPeriod;
        StartDate = startDate;
        Price = price;
        AutoRenew = autoRenew;
        TrialDays = trialDays;
        
        if (trialDays.HasValue && trialDays.Value > 0)
        {
            Status = SubscriptionStatus.Trial;
            TrialEndDate = startDate.AddDays(trialDays.Value);
            EndDate = TrialEndDate.Value;
        }
        else
        {
            Status = SubscriptionStatus.Active;
            EndDate = CalculateEndDate(startDate, billingPeriod);
        }
        
        NextBillingDate = CalculateNextBillingDate();
    }

    public void Renew()
    {
        if (Status == SubscriptionStatus.Trial && TrialEndDate.HasValue)
        {
            // Transition from trial to active
            Status = SubscriptionStatus.Active;
            StartDate = TrialEndDate.Value;
        }
        else
        {
            StartDate = EndDate;
        }
        
        EndDate = CalculateEndDate(StartDate, BillingPeriod);
        NextBillingDate = CalculateNextBillingDate();
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

        return EndDate;
    }

    public void UpdateBillingPeriod(BillingPeriod newPeriod, decimal newPrice)
    {
        BillingPeriod = newPeriod;
        Price = newPrice;
        EndDate = CalculateEndDate(StartDate, newPeriod);
        NextBillingDate = CalculateNextBillingDate();
    }
}
