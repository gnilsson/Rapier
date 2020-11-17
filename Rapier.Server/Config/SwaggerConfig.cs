using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Rapier.Server.Config
{
    public static class SwaggerConfig
    {
        public static void ConfigureSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerSettings = new SwaggerSettings().Bind(configuration);
            app.UseSwagger(option => { option.RouteTemplate = swaggerSettings.JsonRoute; });
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerSettings.UIEndpoint, swaggerSettings.Description);
            });
        }
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Rapier.Server API", Version = "v1" });

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the bearer scheme",
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id ="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }

    public class OpFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var abc = new List<OpenApiTag> { new OpenApiTag { Description = "grpA" } };
            throw new System.NotImplementedException();
        }
    }
}
