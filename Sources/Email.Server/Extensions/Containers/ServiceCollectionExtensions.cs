

namespace Email.Server.Extensions.Containers;

public static class ServiceCollectionExtensions
{


    public static IServiceCollection AddCustomHsts(this IServiceCollection services)
    {
        // HSTS
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        return services;
    }

    // Add Custom Core Service
    public static IServiceCollection AddCustomCoreServices(this IServiceCollection services)
    {


        return services;
    }

}