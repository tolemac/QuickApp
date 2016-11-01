using Microsoft.Extensions.DependencyInjection;

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

        public static QuickApplication AddMongoService(this QuickApplication quickApp, string serviceName = null)
        {
            if (serviceName == null)
                serviceName = "mongodb";
            quickApp.AddService(typeof(IMongoDbDatabaseService), serviceName,
                () => quickApp.ServiceProvider.GetService<IMongoDbDatabaseService>());
            return quickApp;
        }
    }
}
