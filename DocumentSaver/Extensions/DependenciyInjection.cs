using DocumentSaver.Authorization;
using DocumentSaver.Services;

namespace DocumentSaver.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services)
        {

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<IJwtUtils, JwtUtils>();

            return services;

        }

    }
}
