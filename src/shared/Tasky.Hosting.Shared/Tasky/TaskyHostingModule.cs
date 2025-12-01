using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StackExchange.Redis;
using Tasky.MultiTenancy;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Swashbuckle;

namespace Tasky;

[DependsOn(typeof(AbpAspNetCoreMultiTenancyModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpAutofacModule))]
// [DependsOn(typeof(AbpBackgroundJobsRabbitMqModule))]  // Optional: Disabled for local development
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]  // Enabled: Using local Redis
[DependsOn(typeof(AbpDataModule))]
[DependsOn(typeof(AbpDistributedLockingModule))]  // Required by OpenIddict
// [DependsOn(typeof(AbpEventBusRabbitMqModule))]  // Optional: Disabled for local development
[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(TaskySharedModule))]
public class TaskyHostingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        // Configure Redis cache
        ConfigureRedisCache(context, configuration);

        // Configure Redis-based distributed locking for local development
        ConfigureDistributedLocking(context, configuration);

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("is", "is", "Icelandic"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });

        Configure<AbpRabbitMqOptions>(options =>
        {
            // Note: RabbitMQ is optional for local development
            // var cstr = configuration.GetConnectionString(TaskyNames.RabbitMq);
            // options.Connections.Default = new ConnectionFactory() { Uri = new Uri(cstr!) };
        });

        Configure<AbpRabbitMqEventBusOptions>(options =>
        {
            // Note: RabbitMQ is optional for local development
            // options.ClientName = configuration["RabbitMQ:EventBus:ClientName"]!;
            // options.ExchangeName = configuration["RabbitMQ:EventBus:ExchangeName"]!;
        });
    }

    private static void ConfigureRedisCache(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        var redisConnection = configuration.GetConnectionString(TaskyNames.Redis) ?? "localhost:6379";
        
        context.Services.Configure<RedisCacheOptions>(options =>
        {
            options.Configuration = redisConnection;
        });
    }

    private static void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            // Default to localhost:6379 if not configured
            var connectionString = configuration.GetConnectionString(TaskyNames.Redis) ?? "localhost:6379";
            var connection = ConnectionMultiplexer.Connect(connectionString);

            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }
}

public static class HostingExtensions
{
    public static ServiceConfigurationContext ConfigureDataProtection(
        this ServiceConfigurationContext context,
        IWebHostEnvironment hostingEnvironment,
        IConfiguration configuration,
        string name
    )
    {
        var dataProtectionBuilder = context
            .Services.AddDataProtection()
            .SetApplicationName(TaskyNames.Tasky);
        
        // Use Redis for data protection keys in all environments (localhost for development)
        var connectionString = configuration.GetConnectionString(TaskyNames.Redis) ?? "localhost:6379";
        var redis = ConnectionMultiplexer.Connect(connectionString);
        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, $"{name}-Keys");

        return context;
    }
}
