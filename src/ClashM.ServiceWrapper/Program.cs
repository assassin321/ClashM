using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "ClashM Elevated Service";
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ElevatedProcessManager>();
    })
    .Build();

await host.RunAsync();