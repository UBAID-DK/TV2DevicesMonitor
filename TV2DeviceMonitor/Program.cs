// using System;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using Prometheus;
// using TV2DeviceMonitor.Core.Interfaces;
// using TV2DeviceMonitor.Core.Services;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Builder;
// using System.Threading.Tasks;


// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("TV2DeviceMonitor.Tests")]

// public partial class Program
// {
//     /// <summary>
//     /// Entry point of the application. Configures services, starts Prometheus metrics server, and runs the device monitoring host.
//     /// </summary>
//     /// <param name="args">Command-line arguments passed to the application.</param>
//     public static async Task Main(string[] args)
//     {
//         var builder = Host.CreateApplicationBuilder(args);

//         // Your existing config and services registration
//         builder.Services.AddHttpClient<IApiClient, ApiClient>();
//         builder.Services.AddSingleton<IMetricsService, MetricsService>();
//         builder.Services.AddHostedService<DeviceMonitorService>();

//         var host = builder.Build();

//         // Start Prometheus metric server on port 1234
//         var metricServer = new MetricServer(port: 5000);
//         metricServer.Start();

//         await host.RunAsync();
//     }
// }
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

        // Prometheus middleware
        app.UseRouting();
        app.UseHttpMetrics(); // collects default metrics

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics(); // exposes /metrics
        });

        await app.RunAsync();
    }
}