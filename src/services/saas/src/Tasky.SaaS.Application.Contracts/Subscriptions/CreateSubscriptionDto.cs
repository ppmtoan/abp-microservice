using System;
using System.ComponentModel.DataAnnotations;
using Tasky.SaaS.Enums;

namespace Tasky.SaaS.Subscriptions;

public class CreateSubscriptionDto
{
    public Guid? TenantId { get; set; }
    
    [Required]
    public Guid EditionId { get; set; }
    
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool AutoRenew { get; set; } = true;
    
    public int? TrialDays { get; set; }
}
