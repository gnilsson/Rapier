using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rapier.Configuration;
using Rapier.Server.Config;
using Rapier.Server.Data;
using System;
using System.Collections.Generic;

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
                opt.Routes = new Dictionary<Type, string>
                {
                    { typeof(Blog), "api/blogs" },
                    { typeof(Post), "api/posts" }
                };

            });

            services.AddDbContext<RapierDbContext>(options =>
                options.UseSqlServer(Configuration
                    .GetConnectionString("sqlConn")));

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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
