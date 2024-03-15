using AudioGeraImagemWorker.Worker.Events;
using MassTransit;
using Quartz;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class BusConfiguration
    {
        public static void AddBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var fila = configuration.GetSection("MassTransit")["NomeFila"] ?? string.Empty;
            var filaRetentativa = configuration.GetSection("MassTransit")["NomeFilaRetentativa"] ?? string.Empty;
            var servidor = configuration.GetSection("MassTransit")["Servidor"] ?? string.Empty;
            var usuario = configuration.GetSection("MassTransit")["Usuario"] ?? string.Empty;
            var senha = configuration.GetSection("MassTransit")["Senha"] ?? string.Empty;

            //services.AddQuartz(q =>
            //{
            //    q.UseMicrosoftDependencyInjectionJobFactory();

            //    q.UseDefaultThreadPool(tp =>
            //    {
            //        tp.MaxConcurrency = 10;
            //    });

            //    q.UseTimeZoneConverter();

            //});

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, "/", h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.ReceiveEndpoint(fila, e =>
                    {
                        e.ConfigureConsumer<NovoComandoConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(filaRetentativa, e =>
                    {
                        e.ConfigureConsumer<RetentativaComandoConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<NovoComandoConsumer>();
                x.AddConsumer<RetentativaComandoConsumer>();
            });

            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
            });

            //services.AddQuartzHostedService(options =>
            //{
            //    options.StartDelay = TimeSpan.FromSeconds(5);
            //    options.WaitForJobsToComplete = true;
            //});
        }
    }
}