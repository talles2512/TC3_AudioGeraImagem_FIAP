using AudioGeraImagemWorker.Worker.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class BusConfiguration
    {
        public static void AddBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var fila = configuration.GetSection("MassTransit")["NomeFila"] ?? string.Empty;
            var servidor = configuration.GetSection("MassTransit")["Servidor"] ?? string.Empty;
            var usuario = configuration.GetSection("MassTransit")["Usuario"] ?? string.Empty;
            var senha = configuration.GetSection("MassTransit")["Senha"] ?? string.Empty;

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, "/", h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.ReceiveEndpoint(fila, e =>
                    {
                        e.Consumer<ComandoConsumer>();
                    });

                    cfg.ConfigureEndpoints(context);
                });

                config.AddConsumer<ComandoConsumer>();
            });
        }
    }
}
