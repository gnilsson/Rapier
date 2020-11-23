using Microsoft.Extensions.DependencyInjection;

namespace Rapier.Server.Config
{
    public static class CorsConfig
    {
        public static void AddCors(this IServiceCollection services, string cors)
        {
            services.AddCors(policy =>
            {
                policy.AddPolicy(cors, opt => opt
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
        }
    }
}
