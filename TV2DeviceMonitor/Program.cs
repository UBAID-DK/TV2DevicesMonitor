using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TV2DeviceMonitor.Core.Interfaces;
using TV2DeviceMonitor.Core.Services;

public partial class Program
{
    /// <summary>
    /// Entry point of the application. Configures services, starts Prometheus metrics server, and runs the device monitoring host.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpClient<IApiClient, ApiClient>();
        builder.Services.AddSingleton<IMetricsService, MetricsService>();
        builder.Services.AddHostedService<DeviceMonitorService>();

        var app = builder.Build();

        // Prometheus middleware (no need for UseRouting anymore)
        app.UseHttpMetrics(); // collects default metrics

        // ✅ Modern route registration
        app.MapMetrics(); // exposes /metrics

        await app.RunAsync();
    }
}