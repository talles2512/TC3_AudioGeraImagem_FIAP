using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI;
using Microsoft.Extensions.DependencyInjection;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class ParametersConfiguration
    {
        public static void AddParameters(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetRequiredSection("OpenAI").Get<OpenAIParameters>());
        }
    }
}
