using System;
using System.ComponentModel.DataAnnotations;
using Tasky.SaaS.Enums;

namespace Tasky.SaaS.TenantProvisioning;

public class TenantProvisioningRequestDto
{
    [Required]
    [StringLength(64)]
    public string TenantName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string AdminEmail { get; set; }
    
    [Required]
    [StringLength(128)]
    public string AdminPassword { get; set; }
    
    [StringLength(128)]
    public string AdminUserName { get; set; }
    
    [Required]
    public Guid EditionId { get; set; }
    
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    public int? TrialDays { get; set; }
}
