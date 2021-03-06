using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace Miru.Foundation.Logging
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAppLogger<TStartup>(this IServiceCollection services)
        {
            return services.AddSingleton(new AppLoggerFactory(Log.ForContext<TStartup>));
        }

        public static IServiceCollection AddSerilogConfig(this IServiceCollection services, Action<LoggerConfiguration> config = null)
        {
            if (config == null)
                services.AddSingleton<ILoggerConfigurationBuilder>(new LoggerConfigurationBuilder(_ => { }));
            else
                services.AddSingleton<ILoggerConfigurationBuilder>(new LoggerConfigurationBuilder(config));
            
            services.TryAddSingleton(sp =>
            {
                var loggerConfiguration = new LoggerConfiguration();
        
                var configBuilders = sp.GetServices<ILoggerConfigurationBuilder>();
        
                foreach (var configBuilder in configBuilders)
                {
                    configBuilder.Config(loggerConfiguration);
                }
                
                var logger = loggerConfiguration.CreateLogger();
        
                return new RegisteredLogger(logger);
            });   
                
            services.ReplaceSingleton<ILoggerFactory>(sp =>
            {
                var logger = sp.GetRequiredService<RegisteredLogger>().Logger;
                
                Log.Logger = logger;

                var factory = new SerilogLoggerFactory(logger, true);

                return factory;    
            });
            
            return services;
        }
    }
}