using Microsoft.Extensions.DependencyInjection;

namespace QuickApp
{
    public static class ServiceCollectionExtensions
    {
        public static QuickAppConfiguration AddQuickApp(this IServiceCollection serviceCollection)
        {
            var app = new QuickAppConfiguration();
            serviceCollection.AddSingleton(provider => app);
            return app;
        }
    }
}
