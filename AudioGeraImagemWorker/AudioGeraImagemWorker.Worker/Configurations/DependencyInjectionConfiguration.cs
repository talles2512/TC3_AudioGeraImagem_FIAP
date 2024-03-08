using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Application.Services;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services;
using AudioGeraImagemWorker.Infra.Repositories;
using AudioGeraImagemWorker.Infra.Utility;
using AudioGeraImagemWorker.Infra.Utility.Vendor;
using AudioGeraImagemWorker.Worker.Events;
using Polly;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            // Worker
            services.AddScoped<ComandoConsumer>();
            // Application
            services.AddScoped<IEventReceiver, EventReceiver>();
            // Domain
            services.AddScoped<IComandoManager, ComandoManager>();
            services.AddScoped<IErroManager, ErroManager>();
            // Domain - Vendors
            services.AddScoped<IBucketManager, BucketManager>();
            services.AddScoped<IOpenAIVendor, OpenAIVendor>();
            //Repository
            services.AddScoped<IComandoRepository, ComandoRepository>();
            //Utility HttpClient
            services.AddScoped<HttpHelp>();
            services.AddSingleton<AsyncPolicy>(
               PollyConfiguration.CreateWaitAndRetryPolicy(new[]
               {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3)
               }));
        }
    }
}