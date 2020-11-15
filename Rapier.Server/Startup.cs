using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rapier.Configuration;
using Rapier.Configuration.Settings;
using Rapier.External.Enums;
using Rapier.Server.Authorization;
using Rapier.Server.Config;
using Rapier.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRapierControllers(opt =>
            {
                opt.ContextType = typeof(RapierDbContext);
                opt.AssemblyType = typeof(Startup);

                var blogEndpoint = new ControllerEndpointSettings
                {
                    Route = "api/blogs",
                    //AuthorizeableEndpoint = new()
                    //{
                    //    Category = AuthorizationCategory.Default,
                    //    Policy = "WorksForRapier"
                    //}

                };

                //blogEndpoint.ActionSettingsCollection
                //    .FirstOrDefault(x => x.ActionMethod == "Get")
                //    .AuthorizeableEndpoint.Category = AuthorizationCategory.Custom;

                opt.EndpointSettingsCollection = new Dictionary<Type, ControllerEndpointSettings>
                {
                    { typeof(Blog), blogEndpoint },
                    { typeof(Post), new ControllerEndpointSettings { Route="api/posts" } },
                    { typeof(Author), new ControllerEndpointSettings { Route="api/authors" } },
                };

            });

            services.AddDbContext<RapierDbContext>(options =>
                options.UseSqlServer(Configuration
                    .GetConnectionString("sqlConn"))
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging());

            services.AddJwt(Configuration);
            services.AddAuthorizationHandlers();
            services.AddRapier();
            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.AddRapierExceptionMiddleware();
            app.ConfigureSwagger(Configuration);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
