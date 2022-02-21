using FrwkQuickWait.Domain.Intefaces;
using FrwkQuickWait.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrwkQuickWait.Infrastructure.IOC
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
          => services
                .AddScoped<IConsumerService, ConsumerService>()
                .AddScoped<IProducerService, ProducerService>();

    }
}
