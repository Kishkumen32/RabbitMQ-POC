using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Linq;
using System.Reflection;
using Infrastructure.AspNet.Extensions;
using Core.Handlers;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRedis(options => 
            {
                Configuration.GetSection("Redis").Bind(options);
            });

            services.AddHealthChecks();
            services.AddControllers()
                    .AddControllersAsServices();

            services.AddMediatR(Assembly.GetExecutingAssembly(),
                                typeof(WhoAmIHandler).Assembly);

            services.AddSwaggerGen();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var controllerTypesInAssembly = typeof(Startup).Assembly
                                                           .GetExportedTypes()
                                                           .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                                                           .ToArray();

            builder.RegisterTypes(controllerTypesInAssembly)
                   .PropertiesAutowired();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
