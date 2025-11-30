using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddSharedEndpoints(this IHostApplicationBuilder builder)
    {
        // Note: RabbitMQ and Seq are optional. Redis is enabled for local development.
        
        // builder.AddRabbitMQClient(
        //     connectionName: TaskyNames.RabbitMq,
        //     action =>
        //         action.ConnectionString = builder.Configuration.GetConnectionString(
        //             TaskyNames.RabbitMq
        //         )
        // );
        
        // Use localhost:6379 as default for Redis
        var redisConnection = builder.Configuration.GetConnectionString(TaskyNames.Redis) ?? "localhost:6379";
        builder.AddRedisClient(connectionName: TaskyNames.Redis, configureSettings: settings =>
        {
            settings.ConnectionString = redisConnection;
        });
        
        // builder.AddSeqEndpoint(connectionName: TaskyNames.Seq);

        return builder;
    }
}
