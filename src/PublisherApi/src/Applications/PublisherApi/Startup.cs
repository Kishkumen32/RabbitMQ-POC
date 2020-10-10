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
using System;
using System.Linq;
using System.Reflection;
using Infrastructure.AspNet.Extensions;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRedis(options => 
            {
                Configuration.GetSection("Redis").Bind(options);
            });

            services.AddHealthChecks();
            services.AddControllers();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddSwaggerGen();

            var builder = new ContainerBuilder();

            builder.Populate(services);

            var controllerTypesInAssembly = typeof(Startup).Assembly
                                                           .GetExportedTypes()
                                                           .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                                                           .ToArray();

            builder.RegisterTypes(controllerTypesInAssembly)
                   .PropertiesAutowired();

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

