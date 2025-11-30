using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tasky.SaaS.Editions;

public class UpdateEditionDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(256)]
    public string DisplayName { get; set; }
    
    [StringLength(1024)]
    public string Description { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal YearlyPrice { get; set; }
    
    public bool IsActive { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public Dictionary<string, object> FeatureLimits { get; set; }
}
