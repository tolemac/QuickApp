using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = QuickApp.Services.ServiceDescriptor;

namespace QuickApp.MongoDb
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuickAppMongoService(this IServiceCollection serviceCollection, string connectionString,
            string databaseName)
        {
            serviceCollection.AddScoped<IMongoDbDatabaseService>(
                provider => new MongoDbDatabaseService(connectionString, databaseName));

            return serviceCollection;
        }

        public static QuickApplication AddMongoService(this QuickApplication quickApp, string serviceName = null, 
            Action<ServiceDescriptor> configureService = null)
        {
            if (serviceName == null)
                serviceName = "mongodb";
            quickApp.AddService(new Services.ServiceDescriptor(serviceName,
                typeof(IMongoDbDatabaseService),
                () => quickApp.ServiceProvider.GetService<IMongoDbDatabaseService>()
            ), configureService);
            return quickApp;
        }
    }
}
