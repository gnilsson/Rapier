using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Rapier.Server.Authorization;
using Rapier.Server.Descriptive;

namespace Rapier.Server.Config
{
    public static class AuthorizationConfig
    {
        public static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.WorksForRapier, policy =>
                {
                    policy.AddRequirements(new WorksForCompanyRequirement("RapierDemoCompany"));
                });
            });
            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();
        }
    }
}
