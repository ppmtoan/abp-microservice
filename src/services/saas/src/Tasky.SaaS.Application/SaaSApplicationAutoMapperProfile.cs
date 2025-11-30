using AutoMapper;
using Tasky.SaaS.Editions;
using Tasky.SaaS.Entities;
using Tasky.SaaS.Invoices;
using Tasky.SaaS.Subscriptions;

namespace Tasky.SaaS;

public class SaaSApplicationAutoMapperProfile : Profile
{
    public SaaSApplicationAutoMapperProfile()
    {
        CreateMap<Edition, EditionDto>()
            .AfterMap((src, dest) =>
            {
                dest.FeatureLimits = string.IsNullOrEmpty(src.FeatureLimits)
                    ? new System.Collections.Generic.Dictionary<string, object>()
                    : System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(src.FeatureLimits);
            });
        
        CreateMap<CreateEditionDto, Edition>()
            .AfterMap((src, dest) =>
            {
                dest.FeatureLimits = src.FeatureLimits == null
                    ? "{}"
                    : System.Text.Json.JsonSerializer.Serialize(src.FeatureLimits);
            });
        
        CreateMap<UpdateEditionDto, Edition>()
            .AfterMap((src, dest) =>
            {
                dest.FeatureLimits = src.FeatureLimits == null
                    ? "{}"
                    : System.Text.Json.JsonSerializer.Serialize(src.FeatureLimits);
            });

        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.TenantName, opt => opt.Ignore())
            .ForMember(dest => dest.EditionName, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.DaysRemaining = (src.EndDate - System.DateTime.UtcNow).Days;
            });
        
        CreateMap<CreateSubscriptionDto, Subscription>();

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.TenantName, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.IsOverdue = src.Status == Enums.InvoiceStatus.Overdue || 
                    (src.Status == Enums.InvoiceStatus.Pending && System.DateTime.UtcNow > src.DueDate);
            });
    }
}
