using AutoMapper;
using Tasky.SaaS.Editions;
using Tasky.SaaS.Entities;
using Tasky.SaaS.Invoices;
using Tasky.SaaS.Subscriptions;
using Tasky.SaaS.ValueObjects;

namespace Tasky.SaaS;

public class SaaSApplicationAutoMapperProfile : Profile
{
    public SaaSApplicationAutoMapperProfile()
    {
        // Edition mappings
        CreateMap<Edition, EditionDto>()
            .ForMember(dest => dest.MonthlyPrice, opt => opt.MapFrom(src => src.MonthlyPrice.Amount))
            .ForMember(dest => dest.YearlyPrice, opt => opt.MapFrom(src => src.YearlyPrice.Amount))
            .ForMember(dest => dest.FeatureLimits, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.FeatureLimits = new System.Collections.Generic.Dictionary<string, object>
                {
                    ["MaxUsers"] = src.FeatureLimits.MaxUsers,
                    ["MaxProjects"] = src.FeatureLimits.MaxProjects,
                    ["StorageQuotaGB"] = src.FeatureLimits.StorageQuotaGB,
                    ["APICallsPerMonth"] = src.FeatureLimits.APICallsPerMonth,
                    ["EnableAdvancedReports"] = src.FeatureLimits.EnableAdvancedReports,
                    ["EnablePrioritySupport"] = src.FeatureLimits.EnablePrioritySupport,
                    ["EnableCustomBranding"] = src.FeatureLimits.EnableCustomBranding
                };
            });
        
        CreateMap<CreateEditionDto, Edition>()
            .ForMember(dest => dest.MonthlyPrice, opt => opt.Ignore())
            .ForMember(dest => dest.YearlyPrice, opt => opt.Ignore())
            .ForMember(dest => dest.FeatureLimits, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.UpdatePricing(new Money(src.MonthlyPrice), new Money(src.YearlyPrice));
                dest.UpdateFeatureLimits(CreateFeatureLimitsFromDto(src.FeatureLimits));
            });
        
        CreateMap<UpdateEditionDto, Edition>()
            .ForMember(dest => dest.MonthlyPrice, opt => opt.Ignore())
            .ForMember(dest => dest.YearlyPrice, opt => opt.Ignore())
            .ForMember(dest => dest.FeatureLimits, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.UpdatePricing(new Money(src.MonthlyPrice), new Money(src.YearlyPrice));
                dest.UpdateFeatureLimits(CreateFeatureLimitsFromDto(src.FeatureLimits));
            });

        // Subscription mappings
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.SubscriptionPeriod.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.SubscriptionPeriod.EndDate))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.TenantName, opt => opt.Ignore())
            .ForMember(dest => dest.EditionName, opt => opt.Ignore())
            .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => (int)(src.SubscriptionPeriod.EndDate - System.DateTime.UtcNow).TotalDays));
        
        CreateMap<CreateSubscriptionDto, Subscription>()
            .ForMember(dest => dest.Price, opt => opt.Ignore())
            .ForMember(dest => dest.SubscriptionPeriod, opt => opt.Ignore());

        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber.Value))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.BillingPeriodRange.StartDate))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.BillingPeriodRange.EndDate))
            .ForMember(dest => dest.TenantName, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < System.DateTime.UtcNow && src.Status != Enums.InvoiceStatus.Paid));
    }

    private static FeatureLimits CreateFeatureLimitsFromDto(System.Collections.Generic.Dictionary<string, object> dto)
    {
        if (dto == null || dto.Count == 0)
        {
            return FeatureLimits.Free();
        }

        return new FeatureLimits(
            maxUsers: GetIntValue(dto, "MaxUsers", 5),
            maxProjects: GetIntValue(dto, "MaxProjects", 3),
            storageQuotaGB: GetIntValue(dto, "StorageQuotaGB", 10),
            apiCallsPerMonth: GetIntValue(dto, "APICallsPerMonth", 1000),
            enableAdvancedReports: GetBoolValue(dto, "EnableAdvancedReports", false),
            enablePrioritySupport: GetBoolValue(dto, "EnablePrioritySupport", false),
            enableCustomBranding: GetBoolValue(dto, "EnableCustomBranding", false)
        );
    }

    private static int GetIntValue(System.Collections.Generic.Dictionary<string, object> dict, string key, int defaultValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return System.Convert.ToInt32(value);
        }
        return defaultValue;
    }

    private static bool GetBoolValue(System.Collections.Generic.Dictionary<string, object> dict, string key, bool defaultValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return System.Convert.ToBoolean(value);
        }
        return defaultValue;
    }
}
