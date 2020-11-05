using InertiaAdapter.Core;
using InertiaAdapter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InertiaAdapter.Extensions
{
    public static class ConfigureServices
    {

        public static IServiceCollection AddInertia(this IServiceCollection services)
        {

            services.AddScoped<IResultFactory, ResultFactory>();

            return services;
        }
    }
}