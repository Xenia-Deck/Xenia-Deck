using Microsoft.Extensions.DependencyInjection;
using XeniaManagerDatabase.Services;

namespace XeniaManagerDatabase.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddXbox360Database(this IServiceCollection services)
        {
            services.AddHttpClient<IXbox360DatabaseService, Xbox360DatabaseService>(x =>
            {
                x.BaseAddress = new Uri("https://xenia-manager.github.io/x360db/");
            });

            return services;
        }
    }
}
