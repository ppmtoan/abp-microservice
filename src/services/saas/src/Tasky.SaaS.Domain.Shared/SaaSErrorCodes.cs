namespace Tasky.SaaS;

public static class SaaSErrorCodes
{
    public const string EditionNotFound = "SaaS:001";
    public const string SubscriptionNotFound = "SaaS:002";
    public const string InvoiceNotFound = "SaaS:003";
    public const string TenantNotAvailable = "SaaS:004";
    public const string TenantAlreadyExists = "SaaS:005";
    public const string InvalidBillingPeriod = "SaaS:006";
    public const string SubscriptionExpired = "SaaS:007";
    public const string InvoiceAlreadyPaid = "SaaS:008";
}
