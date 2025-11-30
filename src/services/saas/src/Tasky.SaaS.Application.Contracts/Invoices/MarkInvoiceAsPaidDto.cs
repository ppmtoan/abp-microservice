using System.ComponentModel.DataAnnotations;

namespace Tasky.SaaS.Invoices;

public class MarkInvoiceAsPaidDto
{
    [StringLength(128)]
    public string PaymentMethod { get; set; }
    
    [StringLength(256)]
    public string PaymentReference { get; set; }
}
