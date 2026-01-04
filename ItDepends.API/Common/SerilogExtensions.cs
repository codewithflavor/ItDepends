using Serilog;
using Serilog.Events;

namespace ItDepends.API.Common;

public static class SerilogExtensions
{
    public static IHostBuilder ConfigureApiSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((_, _, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ItDepends.API")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:O} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        });

        return hostBuilder;
    }
}

