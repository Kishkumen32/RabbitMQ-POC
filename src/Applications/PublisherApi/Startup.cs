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
using Core.Settings;

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

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfiguration = new RabbitMq();

                    Configuration.GetSection("RabbitMq").Bind(rabbitMqConfiguration);

                    cfg.Host(rabbitMqConfiguration.HostAddress, rabbitMqConfiguration.VirtualHost, (cfg) =>
                    {
                        cfg.Username(rabbitMqConfiguration.Username);
                        cfg.Password(rabbitMqConfiguration.Password);
                    });
                    cfg.ConfigureEndpoints(context); 
                });

                x.AddRequestClient<IWhoAmICommand>();
            });

            services.AddMassTransitHostedService();

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
