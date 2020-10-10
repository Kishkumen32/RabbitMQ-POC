using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using Utilities.Settings;

namespace Utilities.AspNet.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureRedis(this IServiceCollection services, Action<Redis> setup)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }

            var redisOptions = new Redis();

            setup(redisOptions);

            var connectionString = redisOptions.ConnectionString;

            var redis = ConnectionMultiplexer.Connect(connectionString);

            services.AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }
    }
}