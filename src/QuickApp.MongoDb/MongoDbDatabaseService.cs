using System.Collections.Generic;
using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuickApp.MongoDb
{
    public class MongoDbDatabaseService : IMongoDbDatabaseService
    {
        private readonly IMongoDatabase _db;
        public MongoDbDatabaseService(string connectionString, string dataBaseName)
        {
            var client = new MongoClient(connectionString);
            //var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase(dataBaseName);
        }

        private BsonDocument Bson(dynamic obj)
        {
            BsonDocument result = null;
            if (obj != null)
            {
                result = BsonDocument.Parse(JsonConvert.SerializeObject(obj));
                if (result.Contains("_id"))
                    result["_id"] = ObjectId.Parse(obj._id.ToString());
            }
            return result;
            //return obj == null? null : BsonDocument.Parse(JsonConvert.SerializeObject(obj));
        }

        public void InsertOne(string collectionName, dynamic document)
        {
            _db.GetCollection<BsonDocument>(collectionName).InsertOne(Bson(document));
        }

        public void InsertMany(string collectionName, IEnumerable<dynamic> documents)
        {
            var list = new List<BsonDocument>();
            foreach (var document in documents)
            {
                list.Add(Bson(document));
            }
            _db.GetCollection<BsonDocument>(collectionName).InsertMany(list);
        }

        public long Count(string collectionName, dynamic filter = null)
        {
            var innerFilter = filter == null? new BsonDocument() : Bson(filter);

            return _db.GetCollection<BsonDocument>(collectionName).Count(new BsonDocumentFilterDefinition<BsonDocument>(innerFilter));
        }

        public dynamic GetById(string collectionName, string id)
        {
            var filter = Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = _db.GetCollection<dynamic>(collectionName).Find(filter).FirstOrDefault();
            result._id = result._id.ToString();
            return result;
        }

        public IList<dynamic> Find(string collectionName, 
            dynamic filter = null, dynamic order = null, dynamic projection = null, 
            int skip = 0, int take = 10)
        {
            var innerFilter = filter == null ? new BsonDocument() : Bson(filter);
            var query = _db.GetCollection<dynamic>(collectionName).Find(new BsonDocumentFilterDefinition<dynamic>(innerFilter));
            if (order != null)
                query = query.Sort(new BsonDocumentSortDefinition<dynamic>(Bson(order)));
            if (projection != null)
                query = query.Project(new BsonDocumentProjectionDefinition<dynamic, dynamic>(Bson(projection)));
            query = query.Skip(skip);
            if (take != - 1)
            {
                query = query.Limit(take);
            }
            var result = query.ToList();
            foreach (var o in result)
            {
                var obj = o as ExpandoObject as IDictionary<string, object>;
                if (obj != null && obj.ContainsKey("_id"))
                    o._id = o._id.ToString();
            }
            return result;
        }

        public dynamic FirstOrNull(string collectionName,
            dynamic filter = null, dynamic order = null, dynamic projection = null)
        {
            var result = Find(collectionName, filter, order, projection, 0, 1) as IList<dynamic>;
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public void UpdateOne(string collectionName, dynamic filter, dynamic update)
        {
            _db.GetCollection<BsonDocument>(collectionName)
                .UpdateOne(new BsonDocumentFilterDefinition<BsonDocument>(Bson(filter)),
                    new BsonDocumentUpdateDefinition<BsonDocument>(Bson(update)));
        }

        public void UpdateMany(string collectionName, dynamic filter, dynamic update)
        {
            _db.GetCollection<BsonDocument>(collectionName)
                .UpdateMany(new BsonDocumentFilterDefinition<BsonDocument>(Bson(filter)),
                    new BsonDocumentUpdateDefinition<BsonDocument>(Bson(update)));
        }

        public void DeleteOne(string collectionName, dynamic filter)
        {
            _db.GetCollection<BsonDocument>(collectionName)
                .DeleteOne(new BsonDocumentFilterDefinition<BsonDocument>(Bson(filter)));
        }

        public void DeleteMany(string collectionName, dynamic filter)
        {
            _db.GetCollection<BsonDocument>(collectionName)
                .DeleteMany(new BsonDocumentFilterDefinition<BsonDocument>(Bson(filter)));
        }
    }
}
