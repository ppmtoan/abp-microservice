using System;
using System.ComponentModel.DataAnnotations;
using Tasky.SaaS.Enums;

namespace Tasky.SaaS.Subscriptions;

public class UpdateSubscriptionDto
{
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool AutoRenew { get; set; }
}
