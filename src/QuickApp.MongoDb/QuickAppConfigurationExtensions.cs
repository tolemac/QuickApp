﻿namespace QuickApp.MongoDb
{
    public static class QuickAppConfigurationExtensions
    {
        public static QuickAppConfiguration AddMongoService(this QuickAppConfiguration app, string connectionString,
            string databaseName, string serviceName = null)
        {
            if (serviceName == null)
                serviceName = "mongodb";
            app.AddService(typeof(IMongoDbDatabaseService), serviceName,
                    () => new MongoDbDatabaseService(connectionString, databaseName));
            return app;
        }
    }
}
