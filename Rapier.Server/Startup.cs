using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rapier.Configuration;
using Rapier.External.Enums;
using Rapier.Server.Config;
using Rapier.Server.Data;
using Rapier.Server.Descriptive;
using Rapier.Server.Responses;
using System;

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

                opt.Add(typeof(Blog), "api/blogs")
                    .ExpandMembersExplicitly(nameof(BlogResponse.Posts))
                    .Authorize(AuthorizationCategory.None, "WorksForRapier")
                    .AuthorizeAction("Delete", AuthorizationCategory.Custom);
                
                opt.Add(typeof(Post), "api/posts");
                opt.Add(typeof(Author), "api/authors");
            });

            services.AddDbContext<RapierDbContext>(options =>
                options.UseSqlServer(Configuration
                    .GetConnectionString("sqlConn"))
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging());

            services.AddCors(ConfigNames.CorsPolicy);
            services.AddJwt(Configuration);
            services.AddAuthorizationHandlers();
            services.AddRapier();
            services.AddSwagger();
            services.AddSwaggerDocument();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.AddRapierExceptionMiddleware();
            app.ConfigureSwagger(Configuration);
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseHttpsRedirection();
            app.UseCors(ConfigNames.CorsPolicy);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
