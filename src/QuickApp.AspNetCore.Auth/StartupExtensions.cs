using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = QuickApp.Services.ServiceDescriptor;

namespace QuickApp.AspNetCore.Auth
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickAppBasicAuth<TUser>(this IServiceCollection serviceCollection, Action<BasicAuthConfiguration<TUser>> configAction)
        {
            var config = new BasicAuthConfiguration<TUser>();
            configAction?.Invoke(config);
            serviceCollection.AddSingleton(provider => config);
            serviceCollection.AddSingleton<BasicCookieAuthentication<TUser>>();
            serviceCollection.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            return serviceCollection;
        }

        public static QuickApplication AddBasicAuthService<TUser>(this QuickApplication quickApp,
            string serviceName = null, Action<ServiceDescriptor> configureService = null)
        {
            if (serviceName == null)
                serviceName = "auth";
            quickApp.AddService(
                new ServiceDescriptor(serviceName, typeof(BasicCookieAuthentication<TUser>),
                    () => quickApp.ServiceProvider.GetService<BasicCookieAuthentication<TUser>>())
                {
                    RequireAuth = false
                }, configureService
            );
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
