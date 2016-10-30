namespace QuickApp.MongoDb
{
    public static class QuickAppExtensions
    {
        public static QuickApplication AddMongoService(this QuickApplication app, string connectionString,
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
