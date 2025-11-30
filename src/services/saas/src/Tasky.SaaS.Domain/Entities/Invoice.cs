using System;
using Tasky.SaaS.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Tasky.SaaS.Entities;

/// <summary>
/// Represents an invoice for a subscription billing period
/// </summary>
public class Invoice : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public Subscription Subscription { get; set; }
    
    public string InvoiceNumber { get; set; }
    
    public DateTime InvoiceDate { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public InvoiceStatus Status { get; set; }
    
    public DateTime? PaidDate { get; set; }
    
    public string PaymentMethod { get; set; }
    
    public string PaymentReference { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateTime PeriodStart { get; set; }
    
    public DateTime PeriodEnd { get; set; }
    
    public string Notes { get; set; }

    protected Invoice()
    {
    }

    public Invoice(
        Guid id,
        Guid? tenantId,
        Guid subscriptionId,
        string invoiceNumber,
        DateTime invoiceDate,
        DateTime dueDate,
        decimal amount,
        BillingPeriod billingPeriod,
        DateTime periodStart,
        DateTime periodEnd) : base(id)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Amount = amount;
        Status = InvoiceStatus.Pending;
        BillingPeriod = billingPeriod;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public void MarkAsPaid(string paymentMethod = null, string paymentReference = null)
    {
        Status = InvoiceStatus.Paid;
        PaidDate = DateTime.UtcNow;
        PaymentMethod = paymentMethod ?? "Manual";
        PaymentReference = paymentReference;
    }

    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate)
        {
            Status = InvoiceStatus.Overdue;
        }
    }

    public void Cancel()
    {
        Status = InvoiceStatus.Cancelled;
    }
}
