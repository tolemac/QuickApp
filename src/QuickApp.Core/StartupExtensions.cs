using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuickApp
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickApp(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<QuickApplication>();
            return serviceCollection;
        }

        private static void RequestHandler(IApplicationBuilder app, QuickApplication quickApp)
        {
            app.Run(async handler =>
            {
                var segments = handler.Request.Path.Value.Split(new[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries);
                var serviceName = segments[1];
                var methodName = segments[2];

                string bodyText = null;

                using (var stream = handler.Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        if (!reader.EndOfStream)
                            bodyText = reader.ReadToEnd();
                    }
                }


                var arguments = bodyText == null
                    ? null
                    : JObject.Parse(bodyText);

                //quickApp.CallServiceMethod(serviceName, methodName, arguments);
                var result = quickApp.CallServiceMethod(serviceName, methodName, arguments);
                if (result != null)
                {
                    await handler.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    handler.Response.ContentType = "application/json; charset=utf-8";
                }
                else
                {
                    handler.Response.StatusCode = 204;
                }

            });
        }

        public static IApplicationBuilder UseQuickApp(this IApplicationBuilder app, string startRouteSegment = "/qa",
            Action<QuickApplication, IApplicationBuilder> configAction = null)
        {
            var quickApp = app.ApplicationServices.GetService<QuickApplication>();
            configAction?.Invoke(quickApp, app);

            if (!startRouteSegment.StartsWith("/"))
                startRouteSegment = "/" + startRouteSegment;

            app.MapWhen(context =>
                    context.Request.Path.StartsWithSegments("/" + startRouteSegment)
                    && context.Request.ContentType.Contains("application/json"),
                qaApp => RequestHandler(qaApp, quickApp));

            return app;
        }
    }
}
