using System;
using System.Collections.Generic;
using Tasky.SaaS.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;

namespace Tasky.SaaS.Entities;

/// <summary>
/// Represents a SaaS Edition (Plan) that defines feature limits and capabilities
/// </summary>
public class Edition : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    
    public string DisplayName { get; set; }
    
    public string Description { get; set; }
    
    public Money MonthlyPrice { get; set; }
    
    public Money YearlyPrice { get; set; }
    
    public bool IsActive { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public FeatureLimits FeatureLimits { get; set; }
    
    public ICollection<Subscription> Subscriptions { get; set; }

    protected Edition()
    {
        Subscriptions = new List<Subscription>();
    }

    public Edition(
        Guid id,
        string name,
        string displayName,
        string description,
        Money monthlyPrice,
        Money yearlyPrice,
        FeatureLimits featureLimits,
        bool isActive = true,
        int displayOrder = 0) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        MonthlyPrice = monthlyPrice;
        YearlyPrice = yearlyPrice;
        FeatureLimits = featureLimits;
        IsActive = isActive;
        DisplayOrder = displayOrder;
        Subscriptions = new List<Subscription>();
    }

    public Money GetPriceForPeriod(Enums.BillingPeriod period)
    {
        return period == Enums.BillingPeriod.Yearly ? YearlyPrice : MonthlyPrice;
    }

    public void UpdatePricing(Money monthlyPrice, Money yearlyPrice)
    {
        MonthlyPrice = monthlyPrice;
        YearlyPrice = yearlyPrice;
    }

    public void UpdateFeatureLimits(FeatureLimits newLimits)
    {
        FeatureLimits = newLimits;
    }
}
