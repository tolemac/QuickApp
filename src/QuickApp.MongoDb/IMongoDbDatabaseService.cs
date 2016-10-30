using System.Collections.Generic;

namespace QuickApp.MongoDb
{
    public interface IMongoDbDatabaseService
    {
        void InsertOne(string collectionName, dynamic document);
        long Count(string collectionName, dynamic filter);
        dynamic Get(string collectionName, string id);
        IEnumerable<dynamic> Find(string collectionName,
            dynamic filter = null, dynamic order = null, dynamic projection = null,
            int skip = 0, int take = 10);
        void UpdateOne(string collectionName, dynamic filter, dynamic update);
        void UpdateMany(string collectionName, dynamic filter, dynamic update);
        void DeleteOne(string collectionName, dynamic filter);
        void DeleteMany(string collectionName, dynamic filter);
    }
}
