using System;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApp
{
    public static class ServiceCollectionExtensions
    {
        public static QuickApplication AddQuickApp(this IServiceCollection serviceCollection, 
            Action<QuickApplication> configureAction = null)
        {
            var app = new QuickApplication();
            serviceCollection.AddSingleton(provider => app);
            configureAction?.Invoke(app);
            return app;
        }
    }
}
