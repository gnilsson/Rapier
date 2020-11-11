using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Rapier.Server.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Config
{
    public static class AuthorizationConfig
    {
        public static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //   options.DefaultPolicy = new AuthorizationPolicy(new List<IAuthorizationRequirement> { new Aut}
                options.AddPolicy("WorksForRapier", policy =>
                {
                    policy.AddRequirements(new WorksForCompanyRequirement("RapierDemoCompany"));
                });
            });
            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();
        }
    }
}
