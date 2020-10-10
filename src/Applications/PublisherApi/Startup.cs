using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Infrastructure.AspNet.Extensions;
using MassTransit;
using Core.Consumers;
using Core.Interfaces;

namespace WebApi
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
            services.ConfigureRedis(options => 
            {
                Configuration.GetSection("Redis").Bind(options);
            });

            services.AddHealthChecks();
            services.AddControllers()
                    .AddControllersAsServices();

            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<WhoAmIConsumer>();
                cfg.AddRequestClient<IWhoAmICommand>();
            });

            //services.AddMassTransit(cfg =>
            //{
            //    cfg.AddConsumer<WhoAmIConsumer>();
            //    cfg.AddRequestClient<IWhoAmI>();
            //});

            services.AddSwaggerGen();
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
