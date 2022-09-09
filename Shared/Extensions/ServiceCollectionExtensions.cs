using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dipterv.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationProvider>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MapperConfigurationExpression>>();

                var configuration = new MapperConfiguration(options.Value);


                //configuration.AssertConfigurationIsValid();
                configuration.CompileMappings();

                return configuration;
            });

            services.Add(new ServiceDescriptor(
                typeof(IMapper),
                sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService),
                ServiceLifetime.Transient));

            return services;
        }

        public static IServiceCollection ConfigureProfile(this IServiceCollection services, Func<Profile> profileCallback)
        {
            services
                .AddOptions<MapperConfigurationExpression>()
                .Configure<IServiceProvider>((options, sp) =>
                {
                    options.AddProfile(profileCallback());
                });

            return services;
        }
    }
}
