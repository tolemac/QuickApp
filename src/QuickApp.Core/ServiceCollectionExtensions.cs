using Microsoft.Extensions.DependencyInjection;

namespace QuickApp
{
    public static class ServiceCollectionExtensions
    {
        public static QuickApplication AddQuickApp(this IServiceCollection serviceCollection)
        {
            var app = new QuickApplication();
            serviceCollection.AddSingleton(provider => app);
            return app;
        }
    }
}
