using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickApp;
using QuickApp.AspNetCore.Auth;
using QuickApp.MongoDb;
using QuickApp.Services.Interceptors;
using System.Linq;

namespace QuickAppWebTest
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add QuickApp
            services.AddQuickApp();
            services.AddQuickAppMongoService("mongodb://localhost:27017", "QuickAppTest");
            services.AddQuickAppBasicAuth<AuthUser>(ConfigAuth);
        }

        private void ConfigAuth(BasicAuthConfiguration<AuthUser> config)
        {
            config
                .SetGetNameFunc(user => user.Name)
                .SetLocateUserByNamePasswordFunc(
                    (provider, name, pass) => UserData.Users.FirstOrDefault(u => u.Name == name && u.Password == pass));
        }

        private void QuickAppConfig(QuickApplication quickApp, IApplicationBuilder app)
        {
            quickApp
                .AddMongoService("mongodb")
                .AddBasicAuthService<AuthUser>("auth")
                .AddInterceptor("mongodb", "InsertOne", Moment.Before, context =>
                {
                    context.Arguments.document.name += " 2";
                    context.Arguments.document.userId = 44;
                })
                .AddInterceptor("mongodb", "Find", Moment.After, context =>
                {
                    context.Result.Add(new { name = "From", surname = "After Find Interceptor" });
                })
                .AddInterceptor("mongodb", new MongoInterceptorImplementingInterface())
                .AddInterceptor("mongodb", new MongoInterceptorUsingMethodsNames());

            app.UseQuickAppBasicAuth();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseQuickApp(configAction: QuickAppConfig, detailedExceptions: env.IsDevelopment());
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                //routes.AddQuickAppRoute("qa");
            });

        }
    }
}
