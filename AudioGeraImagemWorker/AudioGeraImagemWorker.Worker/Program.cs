using AudioGeraImagemWorker.Worker;
using AudioGeraImagemWorker.Worker.Configurations;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;
        if (env.IsDevelopment())
        {
            config.AddJsonFile($"appsettings.Development.json", optional: false, reloadOnChange: true);
        }
        else
        {
            config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        // Dependency Injection
        services.AddDepencyInjection();
        // Serilog
        services.AddSerilogConfiguration(configuration);
        // MassTransit
        services.AddBusConfiguration(configuration);
        // Database Context
        services.AddDbContextConfiguration(configuration);

        services.AddHostedService<Worker>();
    });

hostBuilder.UseSerilogConfiguration();
var host = hostBuilder.Build();
host.Run();
