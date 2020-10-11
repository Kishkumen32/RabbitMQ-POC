using System;
using System.IO;
using System.Threading.Tasks;
using Core.Consumers;
using Core.Settings;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Worker
{
    class Program
    {
        #region Configuration
        public static Action<IConfigurationBuilder> BuildConfiguration =
            builder => builder.SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                              .AddEnvironmentVariables();
        #endregion

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Build())
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .CreateLogger();

            try
            {
                Log.Information("Starting host");

                var host = CreateHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: false, reloadOnChange: true);
                    configHost.AddEnvironmentVariables(prefix: "WORKER_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configApp.AddEnvironmentVariables();
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    services.AddMassTransit(x => 
                    {
                        x.AddConsumersFromNamespaceContaining<WhoAmIConsumer>();
                        x.SetKebabCaseEndpointNameFormatter();
                        x.UsingRabbitMq((context, cfg) => 
                        {
                            var rabbitMqConfiguration = new RabbitMq();

                            hostContext.Configuration.GetSection("RabbitMq").Bind(rabbitMqConfiguration);

                            cfg.Host(rabbitMqConfiguration.HostAddress,rabbitMqConfiguration.VirtualHost, (cfg) => 
                            { 
                                cfg.Username(rabbitMqConfiguration.Username);
                                cfg.Password(rabbitMqConfiguration.Password);
                            });
                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
