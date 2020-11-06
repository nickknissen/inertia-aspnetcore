using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InertiaAdapter.Extensions
{
    public class InertiaBuilder
    {
        public void AddSharedDataResolvers(ISharedDataResolver value)
        {
            Inertia.AddSharedDataResolver(value);
        }

        public IApplicationBuilder ApplicationBuilder { get; }

        public InertiaBuilder(IApplicationBuilder applicationBuilder)
        {
            ApplicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            Inertia.Init(ApplicationBuilder.ResultFactory());

            ApplicationBuilder.UseWhen(IsNewVersion, RefreshVersion());
            ApplicationBuilder.UseWhen(IsNotRedirect, Redirect());
        }
        private static bool IsNotRedirect(HttpContext hc) =>
              hc.IsInertiaRequest() &&
              new[] { "PATCH", "PUT", "DELETE" }.Contains(hc.Request.Method);

        private static bool IsNewVersion(HttpContext hc) =>
            hc.IsInertiaRequest() &&
            hc.Request.Method == "GET" &&
            hc.Request.Headers["X-Inertia-Version"] != Inertia.GetVersion();

        private static Action<IApplicationBuilder> RefreshVersion() =>
            app => app.Run(async context =>
            {
                var temData = app.TempData(context);

                if (temData.Count > 0)
                    temData.Keep();

                await context.Configure409Response().CompleteAsync();
            });

        private static Action<IApplicationBuilder> Redirect() =>
            app => app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    if (new[] { 302, 301 }.Contains(context.Response.StatusCode))
                        context.Response.StatusCode = 303;
                    return Task.CompletedTask;
                });

                await next();
            });

    }

    public static class Configure
    {
        public static IApplicationBuilder UseInertia(this IApplicationBuilder app, Action<InertiaBuilder> configure)
        {
            configure(new InertiaBuilder(app));

            return app;
        }
    }
}