// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace StarStocksWeb.Frameworks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
        {
            services.AddSingleton(p => BindConfigInstance<TConfig>(section));
            return services;
        }

        public static IServiceCollection AddScopedConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
        {
            services.AddScoped(p => BindConfigInstance<TConfig>(section));
            return services;
        }

        public static IServiceCollection AddTransientConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
        {
            services.AddTransient(p => BindConfigInstance<TConfig>(section));
            return services;
        }

        public static IServiceCollection AddConfig<TConfig>(this IServiceCollection services, IConfiguration section, ServiceLifetime lifetime) where TConfig : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(p => BindConfigInstance<TConfig>(section));
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(p => BindConfigInstance<TConfig>(section));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(p => BindConfigInstance<TConfig>(section));
                    break;
                default:
                    throw new UnexpectedEnumValueException($"Value of enum {typeof(ServiceLifetime)}: {nameof(ServiceLifetime)} is not supported.");
            }

            return services;
        }

        private static TConfig BindConfigInstance<TConfig>(IConfiguration section) where TConfig : class
        {
            var instance = Activator.CreateInstance<TConfig>();
            section.Bind(instance);
            return instance;
        }
    }

    public class UnexpectedEnumValueException : Exception
    {
        public UnexpectedEnumValueException(string message) : base(message)
        {
        }
    }
}
