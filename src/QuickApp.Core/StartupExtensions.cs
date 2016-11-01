using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApp
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickApp(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<QuickApplication>();
            return serviceCollection;
        }

        public static IApplicationBuilder UseQuickApp(this IApplicationBuilder app, Action<QuickApplication, IApplicationBuilder> configAction)
        {
            var quickApp = app.ApplicationServices.GetService<QuickApplication>();
            configAction.Invoke(quickApp, app);
            return app;
        }
    }
}
