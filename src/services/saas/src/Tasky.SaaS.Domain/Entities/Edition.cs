using System;
using System.Collections.Generic;
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
    
    public decimal MonthlyPrice { get; set; }
    
    public decimal YearlyPrice { get; set; }
    
    public bool IsActive { get; set; }
    
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Feature limits stored as JSON (e.g., MaxUsers, StorageQuotaGB, EnablePremiumFeature, ApiCallLimit)
    /// </summary>
    public string FeatureLimits { get; set; }
    
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
        decimal monthlyPrice,
        decimal yearlyPrice,
        string featureLimits,
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
}
