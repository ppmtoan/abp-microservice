using System;
using System.ComponentModel.DataAnnotations;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Validation;

namespace Tasky.SaaS.Subscriptions;

public class CreateSubscriptionDto
{
    public Guid? TenantId { get; set; }
    
    [Required]
    public Guid EditionId { get; set; }
    
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    [ValidDateRange(MinDaysFromNow = 0, MaxDaysFromNow = 365, ErrorMessage = "Start date must be within the next 365 days")]
    public DateTime? StartDate { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool AutoRenew { get; set; } = true;
    
    [Range(0, 90, ErrorMessage = "Trial period cannot exceed 90 days")]
    public int? TrialDays { get; set; }
}
