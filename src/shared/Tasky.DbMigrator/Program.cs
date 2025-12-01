using Serilog;
using Tasky.Administration.EntityFrameworkCore;
using Tasky.Projects.EntityFrameworkCore;
using Tasky.SaaS.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Tasky.DbMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TaskyLogging.Initialize();

        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddAppSettingsSecretsJson();

        builder.Logging.AddSerilog();

        builder.Services.AddHostedService<DbMigratorHostedService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
