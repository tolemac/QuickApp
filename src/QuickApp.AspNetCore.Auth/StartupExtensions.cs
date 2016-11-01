using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApp.AspNetCore.Auth
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickAppBasicAuth<TUser>(this IServiceCollection serviceCollection, Action<BasicAuthConfiguration<TUser>> configAction)
        {
            var config = new BasicAuthConfiguration<TUser>();
            configAction?.Invoke(config);
            serviceCollection.AddSingleton(provider => config);
            serviceCollection.AddScoped<BasicCookieAuthentication<TUser>>();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return serviceCollection;
        }

        public static QuickApplication AddBasicAuthService<TUser>(this QuickApplication quickApp, string serviceName = null)
        {
            if (serviceName == null)
                serviceName = "auth";
            quickApp.AddService(typeof(BasicCookieAuthentication<TUser>), serviceName,
                () => quickApp.ServiceProvider.GetService<BasicCookieAuthentication<TUser>>());
            return quickApp;
        }

        public static IApplicationBuilder UseQuickAppBasicAuth(this IApplicationBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = BasicCookieAuthentication.AuthScheme,
                AutomaticAuthenticate = true
            });
            return app;
        }
    }
}
