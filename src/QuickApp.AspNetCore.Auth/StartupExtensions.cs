using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = QuickApp.Services.ServiceDescriptor;

namespace QuickApp.AspNetCore.Auth
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickAppBasicAuth<TUser>(this IServiceCollection serviceCollection,
            Action<BasicAuthConfiguration<TUser>> configAction)
        {
            var config = new BasicAuthConfiguration<TUser>();
            configAction?.Invoke(config);
            serviceCollection.AddSingleton(provider => config);
            serviceCollection.AddSingleton<IBasicCookieAuthentication, BasicCookieAuthentication<TUser>>();
            serviceCollection.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            return serviceCollection;
        }

        public static QuickApplication AddBasicAuthService<TUser>(this QuickApplication quickApp,
            string serviceName = null, Action<ServiceDescriptor> configureService = null)
        {
            if (serviceName == null)
                serviceName = "auth";
            quickApp.AddService(
                new ServiceDescriptor(serviceName, typeof(IBasicCookieAuthentication),
                    () => quickApp.ServiceProvider.GetService<IBasicCookieAuthentication>())
                {
                    RequireAuth = false
                }, configureService
            );
            return quickApp;
        }

        public static IApplicationBuilder UseQuickAppBasicAuth<TUser>(this IApplicationBuilder app, Action<IApplicationBuilder> configOAuth = null)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                AutomaticAuthenticate = true
            });

            configOAuth?.Invoke(app);
            app.MapWhen(context => context.Request.Path.StartsWithSegments("/externalLoginCallback"), conf =>
            {
                conf.Run(async handler =>
                {
                    var returnUrl = handler.Request.Query["returnUrl"].ToString();
                    var httpContext = handler.Request.HttpContext;

                    var authService = (BasicCookieAuthentication<TUser>)conf.ApplicationServices.GetService<IBasicCookieAuthentication>();
                    var configuration = conf.ApplicationServices.GetService<BasicAuthConfiguration<TUser>>();
                    await authService.Logoff();
                    await authService.SignIn(configuration.LocateUserByExternalLogin(conf.ApplicationServices, httpContext.User), true);
                    
                    httpContext.Response.Redirect(returnUrl);
                });
            });

            app.MapWhen(context => context.Request.Path.StartsWithSegments("/externalLogin"),
                conf =>
                {
                    conf.Run(async handler =>
                    {
                        var provider = handler.Request.Query["provider"].ToString();
                        var returnUrl = "/externalLoginCallback?returnUrl=" + handler.Request.Query["returnUrl"].ToString();
                        var httpContext = handler.Request.HttpContext;

                        var authenticationProperties = new AuthenticationProperties()
                        {
                            RedirectUri = returnUrl
                        };

                        //string userId = null;
                        authenticationProperties.Items["LoginProvider"] = provider;
                        //if (userId != null)
                        //    authenticationProperties.Items["XsrfId"] = userId;
                        //return Challenge(authenticationProperties, provider);

                        await httpContext.Authentication.ChallengeAsync(provider, authenticationProperties);
                    });
                });
            return app;
        }
    }
}
