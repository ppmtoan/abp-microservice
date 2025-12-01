using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddSharedEndpoints(this IHostApplicationBuilder builder)
    {
        // Note: Without Aspire, service connections (RabbitMQ, Redis, Seq) are configured 
        // directly in TaskyHostingModule via connection strings in appsettings.json
        
        return builder;
    }
}
